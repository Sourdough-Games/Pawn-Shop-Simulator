using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductSO")]
public class ProductSO : ScriptableObject
{
    public GameObject prefab;

    public string ProductName;

    public string LocalizationKey;

    public float baseCost;

    public ProductType productType;

    public ProductSize productSize;

    public ProductPositionData WorldSlotPositionData;

    public ProductPositionData HandSlotPositionData;

    public bool CanBeSlottedVertically = false;
}
