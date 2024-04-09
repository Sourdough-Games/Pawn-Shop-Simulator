using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : Singleton<ProductManager>
{
    public ProductSO[] Products {
        get {
            return gameProducts;
        }
    }

    [SerializeField] private ProductSO[] gameProducts;

    new void Awake() {
        base.Awake();

        gameProducts = Resources.LoadAll<ProductSO>("Products");
    }
}
