using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
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
}
