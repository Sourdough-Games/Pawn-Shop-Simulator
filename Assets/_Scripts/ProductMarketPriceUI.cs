using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ProductMarketPriceUI : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent productName;
    [SerializeField] private Image productSprite;
    [SerializeField] private TextMeshProUGUI productPrice;

    public string ProductName {
        get {
            return productName.StringReference.GetLocalizedString();
        }
    }

    public void Setup(ProductSO product) {
        productName.StringReference.TableEntryReference = product.LocalizationKey;
        productSprite.sprite = product.sprite;
        productPrice.text = Helper.ConvertToDollarAmountNoCollapse(product.baseCost);
    }
}
