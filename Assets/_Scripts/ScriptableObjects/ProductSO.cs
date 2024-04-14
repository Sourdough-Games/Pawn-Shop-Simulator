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

    public GenericPositionData WorldSlotPositionData;

    public GenericPositionData HandSlotPositionData;

    public bool CanBeSlottedVertically = false;
}
