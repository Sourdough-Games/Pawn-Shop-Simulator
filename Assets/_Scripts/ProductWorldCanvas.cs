using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ProductWorldCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameValueText;
    [SerializeField] private TextMeshProUGUI typeValueText;
    [SerializeField] private TextMeshProUGUI sizeValueText;
    [SerializeField] private TextMeshProUGUI priceValueText;

    private LocalizeStringEvent localizedNameString;
    private LocalizeStringEvent localizedTypeString;
    private LocalizeStringEvent localizedSizeString;

    public bool CanShow = true;

    public Product activeProduct = null;

    void Start() {
        Hide();
    }

    public void Setup(Product product) {
        if(localizedSizeString == null)
        {
            localizedNameString = nameValueText.GetComponent<LocalizeStringEvent>();
            localizedTypeString = typeValueText.GetComponent<LocalizeStringEvent>();
            localizedSizeString = sizeValueText.GetComponent<LocalizeStringEvent>();
        }

        localizedNameString.StringReference.TableEntryReference = product.ProductData.LocalizationKey;
        localizedTypeString.StringReference.TableEntryReference = $"Type{product.ProductData.productType}";
        localizedSizeString.StringReference.TableEntryReference = $"Size{product.ProductData.productSize}";

        nameValueText.text = localizedNameString.StringReference.GetLocalizedString();
        typeValueText.text = localizedTypeString.StringReference.GetLocalizedString();
        sizeValueText.text = localizedSizeString.StringReference.GetLocalizedString();

        activeProduct = product;
    }

    public void Update() {
        if(gameObject.activeSelf && activeProduct != null) {
            priceValueText.text = activeProduct.currentlySetPrice == 0 ? "???" : Helper.ConvertToDollarAmount(activeProduct.currentlySetPrice);
        }
    }

    public void Show() {
        if (!CanShow) return;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
