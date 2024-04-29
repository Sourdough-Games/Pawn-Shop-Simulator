using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PriceLookupUI : Modal
{
    [SerializeField] private GameObject productCard;
    [SerializeField] private Transform Parent;

    [SerializeField] private TMP_InputField filterInput;

    ProductSO[] Products;

    List<ProductMarketPriceUI> elements = new();

    void Start() {
        Products = Resources.LoadAll<ProductSO>("Products");

        foreach(ProductSO product in Products) {
            GameObject obj = Instantiate(productCard, Parent);

            var ui = obj.GetComponent<ProductMarketPriceUI>();

            ui.Setup(product);
            elements.Add(ui);
        }
    }

    new void Open() {
        base.Open();

        filterInput.ActivateInputField();
    }

    public void FilterProducts()
    {
        foreach (var ui in elements)
        {
            if (!ui.ProductName.ToLower().Contains(filterInput.text.ToLower()))
            {
                ui.gameObject.SetActive(false);
                continue;
            }

            ui.gameObject.SetActive(true);
        }
    }

    public override void Draw() {
        
    }
}
