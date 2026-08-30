using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
[AddComponentMenu("Dragon Adventure/Core/Game Controller")]
public class GameController : MonoBehaviour
{
    private const float TransitionCameraSnapDuration = 0.05f;
    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";

    private static GameController instance;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<GameController>();
            return instance;
        }
    }

    [Header("Scene Loading")]
    public string[] levelScenePaths = { "Assets/Scenes/Level 1.unity", "Assets/Scenes/Level 2.unity", "Assets/Scenes/Level 3.unity" };

    [Header("Run State")]
    public int score;

    public bool HasPendingSpawn => !string.IsNullOrWhiteSpace(pendingSpawnId);
    public string PendingSpawnId => pendingSpawnId;

    private string pendingSpawnId;
    private string pendingScenePath;
    private bool transitionInProgress;
    private PlayerController persistentPlayer;
    private Scene currentContentScene;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadInitialContentRoutine());
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void StartGame()
    {
        LoadSceneByPath(levelScenePaths[0], null, false);
    }

    public bool RestartCurrentLevel()
    {
        if (transitionInProgress || !currentContentScene.IsValid() || !currentContentScene.isLoaded)
            return false;

        StartCoroutine(RestartCurrentLevelRoutine(currentContentScene.path));
        return true;
    }

    public bool LoadLevelByOffset(int levelOffset, string spawnId, bool keepPlayerBetweenScenes)
    {
        string currentScenePath = SceneManager.GetActiveScene().path;
        int currentLevelIndex = Array.IndexOf(levelScenePaths, currentScenePath);
        if (currentLevelIndex < 0)
        {
            Debug.LogWarning($"Scene '{currentScenePath}' is not listed in {nameof(GameController)}.{nameof(levelScenePaths)}.");
            return false;
        }

        int targetLevelIndex = currentLevelIndex + levelOffset;
        if (targetLevelIndex < 0 || targetLevelIndex >= levelScenePaths.Length)
        {
            Debug.LogWarning($"No level exists at offset {levelOffset} from '{currentScenePath}'.");
            return false;
        }

        return LoadSceneByPath(levelScenePaths[targetLevelIndex], spawnId, keepPlayerBetweenScenes);
    }

    public bool LoadSceneByBuildOffset(int buildIndexOffset, string spawnId, bool keepPlayerBetweenScenes)
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex < 0)
        {
            Debug.LogWarning("Cannot load scene by offset because the active scene is not in Build Settings.");
            return false;
        }

        return LoadSceneByBuildIndex(currentIndex + buildIndexOffset, spawnId, keepPlayerBetweenScenes);
    }

    public bool LoadSceneByName(string sceneName, string spawnId, bool keepPlayerBetweenScenes)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Cannot load a scene with an empty scene name.");
            return false;
        }

        string scenePath = Array.Find(levelScenePaths, path =>
            string.Equals(Path.GetFileNameWithoutExtension(path), sceneName, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogWarning($"Scene '{sceneName}' is not listed in {nameof(GameController)}.{nameof(levelScenePaths)}.");
            return false;
        }

        return LoadSceneByPath(scenePath, spawnId, keepPlayerBetweenScenes);
    }

    public bool LoadSceneByPath(string scenePath, string spawnId, bool keepPlayerBetweenScenes)
    {
        if (transitionInProgress)
            return false;

        if (string.IsNullOrWhiteSpace(scenePath))
        {
            Debug.LogWarning("Cannot load a scene with an empty path.");
            return false;
        }

        int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Cannot load '{scenePath}'. Ensure it is enabled in Build Settings.");
            return false;
        }

        StartCoroutine(LoadSceneRoutine(scenePath, spawnId, keepPlayerBetweenScenes));
        return true;
    }

    public bool LoadSceneByBuildIndex(int buildIndex, string spawnId, bool keepPlayerBetweenScenes)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"Cannot load scene build index {buildIndex}. Check Build Settings.");
            return false;
        }

        return LoadSceneByPath(SceneUtility.GetScenePathByBuildIndex(buildIndex), spawnId, keepPlayerBetweenScenes);
    }

    public bool ShouldUseSpawn(string spawnId)
    {
        return HasPendingSpawn && pendingSpawnId == spawnId;
    }

    public void ClearPendingSpawn()
    {
        pendingSpawnId = null;
        pendingScenePath = null;
    }

    private IEnumerator LoadSceneRoutine(string scenePath, string spawnId, bool keepPlayerBetweenScenes)
    {
        transitionInProgress = true;
        pendingSpawnId = spawnId;
        pendingScenePath = scenePath;

        Scene previousContentScene = currentContentScene;
        if (keepPlayerBetweenScenes)
            KeepPlayerBetweenScenes();

        if (previousContentScene.IsValid() && previousContentScene.isLoaded)
            SetSceneCamerasEnabled(previousContentScene, false);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        if (loadOperation == null)
        {
            transitionInProgress = false;
            yield break;
        }

        while (!loadOperation.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByPath(scenePath);
        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
            currentContentScene = loadedScene;
        }

        if (HasPendingSpawn)
            PlacePlayerAtPendingSpawn(loadedScene);
        else
            BindHudToPlayer(FindFirstObjectByType<PlayerController>());

        SetSceneCamerasEnabled(loadedScene, true);

        if (previousContentScene.IsValid() && previousContentScene.isLoaded && previousContentScene != loadedScene)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(previousContentScene);
            if (unloadOperation != null)
            {
                while (!unloadOperation.isDone)
                    yield return null;
            }
        }

        transitionInProgress = false;
    }

    private IEnumerator LoadInitialContentRoutine()
    {
        yield return null;
        LoadSceneByPath(MainMenuScenePath, null, false);
    }

    private IEnumerator RestartCurrentLevelRoutine(string scenePath)
    {
        transitionInProgress = true;
        ClearPendingSpawn();

        if (persistentPlayer != null)
        {
            Destroy(persistentPlayer.gameObject);
            persistentPlayer = null;
            yield return null;
        }

        SetSceneCamerasEnabled(currentContentScene, false);
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentContentScene);
        if (unloadOperation != null)
        {
            while (!unloadOperation.isDone)
                yield return null;
        }

        currentContentScene = default;
        transitionInProgress = false;
        LoadSceneByPath(scenePath, null, false);
    }

    private void PlacePlayerAtPendingSpawn(Scene loadedScene)
    {
        if (loadedScene.path != pendingScenePath)
            return;

        LevelSpawnPoint[] spawnPoints = FindObjectsByType<LevelSpawnPoint>(FindObjectsSortMode.None);
        foreach (LevelSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.gameObject.scene != loadedScene || spawnPoint.spawnId != pendingSpawnId)
                continue;

            PlacePlayerAtSpawn(spawnPoint, loadedScene);
            ClearPendingSpawn();
            break;
        }

        if (HasPendingSpawn)
        {
            Debug.LogWarning($"Scene '{loadedScene.path}' has no {nameof(LevelSpawnPoint)} with spawn ID '{pendingSpawnId}'.");
            ClearPendingSpawn();
        }
    }

    private void PlacePlayerAtSpawn(LevelSpawnPoint spawnPoint, Scene loadedScene)
    {
        RemoveDestinationScenePlayers(loadedScene);

        PlayerController player = persistentPlayer;
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.LogWarning($"{nameof(LevelSpawnPoint)} '{spawnPoint.spawnId}' could not find a PlayerController to move.");
            return;
        }

        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        if (playerBody != null)
        {
            playerBody.position = spawnPoint.transform.position;
            playerBody.linearVelocity = Vector2.zero;
        }
        else
        {
            player.transform.position = spawnPoint.transform.position;
        }

        if (spawnPoint.connectMainCameraToPlayer)
            ConnectSceneCameraToPlayer(loadedScene, player.transform, spawnPoint.cameraOffset);

        BindHudToPlayer(player);
    }

    private void BindHudToPlayer(PlayerController player)
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        foreach (HealthBarUI healthBar in FindObjectsByType<HealthBarUI>(FindObjectsSortMode.None))
            healthBar.SetTarget(playerHealth);

        foreach (GameOverUI gameOver in FindObjectsByType<GameOverUI>(FindObjectsSortMode.None))
            gameOver.SetTarget(playerHealth);
    }

    private void ConnectSceneCameraToPlayer(Scene scene, Transform playerTransform, Vector2 cameraOffset)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            Camera sceneCamera = rootObject.GetComponentInChildren<Camera>();
            if (sceneCamera == null || !sceneCamera.CompareTag("MainCamera"))
                continue;

            CameraFollow cameraFollow = sceneCamera.GetComponent<CameraFollow>();
            if (cameraFollow == null)
                cameraFollow = sceneCamera.gameObject.AddComponent<CameraFollow>();

            cameraFollow.target = playerTransform;
            cameraFollow.offset = cameraOffset;
            cameraFollow.SnapToTarget();
            cameraFollow.DisableSmoothingFor(TransitionCameraSnapDuration);
            return;
        }
    }

    private void SetSceneCamerasEnabled(Scene scene, bool enabled)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            foreach (Camera sceneCamera in rootObject.GetComponentsInChildren<Camera>(true))
                sceneCamera.enabled = enabled;
        }
    }

    private void RemoveDestinationScenePlayers(Scene destinationScene)
    {
        foreach (PlayerController player in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
        {
            if (player != persistentPlayer && player.gameObject.scene == destinationScene)
                Destroy(player.gameObject);
        }
    }

    private void KeepPlayerBetweenScenes()
    {
        persistentPlayer = FindFirstObjectByType<PlayerController>();
        if (persistentPlayer != null)
            DontDestroyOnLoad(persistentPlayer.gameObject);
    }
}