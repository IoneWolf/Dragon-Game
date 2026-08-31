using System;
using UnityEngine;

// Tracks enemy HP; destroys the enemy once HP reaches 0 (e.g. from being stomped).
public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Hit points restored when this enemy object is created.")]
    public int maxHP = 1;
    public int CurrentHP { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDefeated;

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHP <= 0 || amount <= 0) return;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
        {
            OnDefeated?.Invoke();
            Destroy(gameObject);
        }
    }
}
