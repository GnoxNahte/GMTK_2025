using UnityEngine;

public static class Utility
{
    public static float Remap(float value, float from1, float from2, float to1, float to2)
    {
        return (value - from1) / (from2 - from1) * (to2 - to1) + to1;
    }
    
    public static float Remap(float value, Vector2 from, Vector2 to)
    {
        return (value - from.x) / (from.y - from.x) * (to.y - to.x) + to.x;
    }

    public static void Shuffle<T>(T[] arr)
    {
        // Shuffle
        int count = arr.Length;
        for (int i = 0; i < count - 1; ++i) {
            int r = Random.Range(i, count);
            (arr[i], arr[r]) = (arr[r], arr[i]);
        }
    }
}

