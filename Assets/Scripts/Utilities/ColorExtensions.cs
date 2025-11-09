using UnityEngine;

public static class ColorExtensions
{
    public static Color ToTransparent(this Color color) {
        return new Color(color.r, color.g, color.b, 0);
    }
    
    public static Color ToOpaque(this Color color) {
        return new Color(color.r, color.g, color.b, 1);
    }
}