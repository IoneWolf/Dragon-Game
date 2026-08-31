using System;
using System.Collections;
using UnityEngine;

// Tracks player HP, applies damage with an invulnerability window, and notifies listeners (UI, feedback).
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Hit points restored when the player object is created.")]
    public int maxHP = 3;
    [Tooltip("Seconds after taking damage during which further damage is ignored.")]
    public float invulnerabilityDuration = 1.5f;
    [Tooltip("Seconds between visibility toggles during the invulnerability flicker.")]
    public float invulnerabilityFlickerInterval = 0.1f;

    public int CurrentHP { get; private set; }
    public bool IsInvulnerable { get; private set; }

    // (currentHP, maxHP) - fired whenever HP changes, so UI (e.g. HealthBarUI) can react.
    public event Action<int, int> OnHealthChanged;
    // Fired every time the player actually takes damage, for feedback ("I got hit").
    public event Action OnPlayerHit;
    public event Action OnPlayerDefeated;

    private PlayerSpriteVisual visual;

    private void Awake()
    {
        CurrentHP = maxHP;
        visual = GetComponentInChildren<PlayerSpriteVisual>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    public bool TakeDamage(int amount)
    {
        if (IsInvulnerable || CurrentHP <= 0 || amount <= 0) return false;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        Debug.Log($"[PlayerHealth] I got hit! HP: {CurrentHP}/{maxHP}");
        OnPlayerHit?.Invoke();
        OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP > 0)
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
        else
        {
            Debug.Log("[PlayerHealth] Player defeated.");
            OnPlayerDefeated?.Invoke();
        }

        return true;
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        IsInvulnerable = true;
        float elapsed = 0f;

        while (elapsed < invulnerabilityDuration)
        {
            visual?.SetVisible(!visual.IsVisible);
            yield return new WaitForSeconds(invulnerabilityFlickerInterval);
            elapsed += invulnerabilityFlickerInterval;
        }

        visual?.SetVisible(true);
        IsInvulnerable = false;
    }
}
