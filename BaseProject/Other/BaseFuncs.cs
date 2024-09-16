using BaseProject.GameManagement;
using Microsoft.Xna.Framework;

namespace BaseProject.Other;

public static class BaseFuncs
{
    public static Color TransitionColor(Color startColor)
    {
        Scene crntScene = GameWorld.Instance.CurrentScene;

        if (crntScene.IsChangingScene)
            return Color.Lerp(startColor, Color.Transparent, (float)crntScene.NormalizedTransitionProgress);
        else
            return startColor;
    }

    public static Vector2 SafeNormalize(Vector2 value)
    {
        float length = value.Length();
        if (length > 0)
        {
            float num = 1f / length;
            value.X *= num;
            value.Y *= num;
        }
        return value;
    }

    public static void SafeNormalize(ref Vector2 value, out Vector2 result)
    {
        float length = value.Length();
        if (length > 0)
        {
            float num = 1f / length;
            result.X = value.X * num;
            result.Y = value.Y * num;
        }
        else
        {
            result = Vector2.Zero;
        }
    }
}
