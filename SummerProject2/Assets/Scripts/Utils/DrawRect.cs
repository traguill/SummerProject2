using UnityEngine;

public static class DrawRect
{
    static Texture2D rect_texture;

    public static Texture2D RectTexture
    {
        get
        {
            if(rect_texture == null)
            {
                rect_texture = new Texture2D(1, 1);
                rect_texture.SetPixel(0, 0, Color.white);
                rect_texture.Apply();
            }

            return rect_texture;
        }
    }

    /// <summary>
    /// Draws a rectangle filled with a given screen rectangle.
    /// </summary>
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, RectTexture);
    }
    /// <summary>
    /// Draws an empty rectangle with a given screen rectangle.
    /// </summary>
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    /// <summary>
    /// Creates a screen position rectangle from two screen positions.
    /// </summary>
    public static Rect GetScreenRect(Vector3 screen_position1, Vector3 screen_position2)
    {
        // Move origin from bottom left to top left
        screen_position1.y = Screen.height - screen_position1.y;
        screen_position2.y = Screen.height - screen_position2.y;

        // Calculate corners
        Vector3 topLeft = Vector3.Min(screen_position1, screen_position2);
        Vector3 bottomRight = Vector3.Max(screen_position1, screen_position2);
        
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screen_position1, Vector3 screen_position2)
    {
        Vector3 v1 = Camera.main.ScreenToViewportPoint(screen_position1);
        Vector3 v2 = Camera.main.ScreenToViewportPoint(screen_position2);

        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);

        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);

        return bounds;
    }
	
}
