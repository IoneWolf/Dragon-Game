using UnityEngine;

// Shared helper for generating a solid-color half-circle placeholder sprite (e.g. a rock) at runtime.
// Dome shape (flat bottom, rounded top), pivoted at the bottom-center so it sits on the ground.
public static class HalfCircleSpriteFactory
{
    public static Sprite CreateHalfCircleSprite(Color color, int width = 64)
    {
        int height = width / 2;
        Texture2D texture = new Texture2D(width, height);
        Color clear = new Color(0f, 0f, 0f, 0f);
        Vector2 center = new Vector2(width / 2f, 0f);
        float radius = width / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 point = new Vector2(x + 0.5f, y + 0.5f);
                bool insideDome = Vector2.Distance(point, center) <= radius;
                texture.SetPixel(x, y, insideDome ? color : clear);
            }
        }
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0f), width);
    }
}
