using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Helper
{
    public static IEnumerator Wait(float time, Action a)
    {
        yield return new WaitForSeconds(time);
        a();
    }
    public static bool IsWithinPlayerReach(Transform obj, float reach) {
        PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

        if(holder == null) return false;

        float distance = Vector3.Distance(obj.position, holder.transform.position);
        return distance <= reach;
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
    private static CultureInfo userCulture = CultureInfo.CurrentCulture;

    public static string ConvertToDollarAmountNoCollapse(float number) {
        return number.ToString("C", userCulture);
    }

    public static float ExponentialBias(float min, float max, float lambda)
    {
        float u = Random.value;
        return Mathf.Lerp(min, max, 1 - Mathf.Pow(1 - u, lambda));
    }

    public static string ConvertToDollarAmount(float number)
    {
        if (number < 999999)
        {
            return number.ToString("C", userCulture); // Currency formatting with the user's locale
        }

        string[] suffixes = { "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "O" };
        float[] thresholds = { 1000000f, 1000000000f, 1000000000000f, 1000000000000000f, 1000000000000000000f, 1000000000000000000000f, 1000000000000000000000000f, 1000000000000000000000000000f };

        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (number >= thresholds[i])
            {
                string formattedValue = (number / thresholds[i]).ToString("F1", userCulture);
                string suffix = suffixes[i];
                string currencySymbol = userCulture.NumberFormat.CurrencySymbol;
                string formattedAmount = formattedValue + suffix;

                if (userCulture.NumberFormat.CurrencyPositivePattern == 2 || userCulture.NumberFormat.CurrencyPositivePattern == 3)
                {
                    // Currency symbol on the left
                    formattedAmount = currencySymbol + formattedAmount;
                }
                else
                {
                    // Currency symbol on the right
                    formattedAmount += currencySymbol;
                }

                return formattedAmount;
            }
        }

        return number.ToString("F0", userCulture); // No conversion needed
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
