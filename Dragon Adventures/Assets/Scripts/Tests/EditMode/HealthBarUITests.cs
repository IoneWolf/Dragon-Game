using NUnit.Framework;
using UnityEngine;

// Unit tests for HealthBarUI's color-thresholding logic (pure static function).
public class HealthBarUITests
{
    [Test]
    public void GetColorForFraction_FullHealth_IsGreen()
    {
        Color color = HealthBarUI.GetColorForFraction(3f / 3f);
        Assert.AreEqual(Color.green, color,
            $"[{nameof(HealthBarUITests)}] Scripts/UI/HealthBarUI.cs: 3/3 HP should be green.");
    }

    [Test]
    public void GetColorForFraction_TwoThirds_IsYellow()
    {
        Color color = HealthBarUI.GetColorForFraction(2f / 3f);
        Assert.AreEqual(Color.yellow, color,
            $"[{nameof(HealthBarUITests)}] Scripts/UI/HealthBarUI.cs: 2/3 HP should be yellow.");
    }

    [Test]
    public void GetColorForFraction_OneThird_IsRed()
    {
        Color color = HealthBarUI.GetColorForFraction(1f / 3f);
        Assert.AreEqual(Color.red, color,
            $"[{nameof(HealthBarUITests)}] Scripts/UI/HealthBarUI.cs: 1/3 HP should be red.");
    }

    [Test]
    public void GetColorForFraction_Zero_IsRed()
    {
        Color color = HealthBarUI.GetColorForFraction(0f);
        Assert.AreEqual(Color.red, color,
            $"[{nameof(HealthBarUITests)}] Scripts/UI/HealthBarUI.cs: 0 HP should be red.");
    }
}
