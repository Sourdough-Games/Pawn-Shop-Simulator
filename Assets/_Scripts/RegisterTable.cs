using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RegisterTable : MonoBehaviour
{
    public UnityEvent LineChanged;

    private List<ShopperNPC> shoppersInLine = new();

    [SerializeField] private Transform customerStandPosition;

    public Transform CustomerStandPosition {
        get {
            return customerStandPosition;
        }
    }

    public void AddShopper(ShopperNPC shopper) {
        shoppersInLine.Add(shopper);
        LineChanged?.Invoke();
    }

    public int GetShopperLinePosition(ShopperNPC shopper) {
        return shoppersInLine.IndexOf(shopper);
    }

    public ShopperNPC GetShopperAtPosition(int position)
    {
        return shoppersInLine[position];
    }

    public void RemoveShopper(ShopperNPC shopper) {
        shoppersInLine.Remove(shopper);
        LineChanged?.Invoke();
    }
}
