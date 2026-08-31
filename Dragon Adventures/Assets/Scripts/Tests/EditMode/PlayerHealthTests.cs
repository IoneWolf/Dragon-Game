using NUnit.Framework;
using UnityEngine;

// Unit tests for PlayerHealth damage/invulnerability behavior.
public class PlayerHealthTests
{
    private GameObject go;
    private PlayerHealth health;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("TestPlayerHealth");
        health = go.AddComponent<PlayerHealth>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void TakeDamage_ReducesHP()
    {
        int before = health.CurrentHP;
        health.TakeDamage(1);
        Assert.AreEqual(before - 1, health.CurrentHP,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: TakeDamage(1) did not reduce HP by 1.");
    }

    [Test]
    public void TakeDamage_SetsInvulnerable()
    {
        health.TakeDamage(1);
        Assert.IsTrue(health.IsInvulnerable,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: TakeDamage did not set IsInvulnerable.");
    }

    [Test]
    public void TakeDamage_WhileInvulnerable_IsIgnored()
    {
        health.TakeDamage(1);
        int hpAfterFirstHit = health.CurrentHP;

        health.TakeDamage(1);

        Assert.AreEqual(hpAfterFirstHit, health.CurrentHP,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: a second hit during invulnerability should be ignored.");
    }

    [Test]
    public void TakeDamage_NeverGoesBelowZero()
    {
        health.TakeDamage(100);
        Assert.AreEqual(0, health.CurrentHP,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: HP should clamp at 0, not go negative.");
    }

    [Test]
    public void RestoreFullHealth_AfterDefeat_RestoresMaximumHP()
    {
        health.TakeDamage(100);

        health.RestoreFullHealth();

        Assert.AreEqual(health.maxHP, health.CurrentHP,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: RestoreFullHealth should restore maximum HP.");
        Assert.IsFalse(health.IsInvulnerable,
            $"[{nameof(PlayerHealthTests)}] Scripts/Player/PlayerHealth.cs: RestoreFullHealth should clear invulnerability.");
    }
}
