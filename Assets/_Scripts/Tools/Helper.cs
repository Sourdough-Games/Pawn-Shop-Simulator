using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static bool IsWithinPlayerReach(Transform obj, float additional_reach = 0) {
        PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

        float distance = Vector3.Distance(obj.position, holder.transform.position);
        return distance <= holder.reachDistance + additional_reach;
    }    
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if(child.GetComponent<Canvas>() != null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    public static string ConvertToDollarAmount(float number)
    {
        if(number < 999999) {
            return number.ToString("C");
        }

        string[] suffixes = { "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "O" };
        float[] thresholds = { 1000000f, 1000000000f, 1000000000000f, 1000000000000000f, 1000000000000000000f, 1000000000000000000000f, 1000000000000000000000000f, 1000000000000000000000000000f };

        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (number >= thresholds[i])
            {
                return (number / thresholds[i]).ToString("F1") + suffixes[i];
            }
        }

        return number.ToString("F0"); // No conversion needed
    }

    public static Vector3 GetRandomPositionWithinBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
