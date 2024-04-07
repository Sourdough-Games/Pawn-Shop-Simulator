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

    private LocalizeStringEvent localizedNameString;
    private LocalizeStringEvent localizedTypeString;
    private LocalizeStringEvent localizedSizeString;

    public bool CanShow = true;

    public void Setup(ProductSO productSO) {
        if(localizedSizeString == null)
        {
            localizedNameString = nameValueText.GetComponent<LocalizeStringEvent>();
            localizedTypeString = typeValueText.GetComponent<LocalizeStringEvent>();
            localizedSizeString = sizeValueText.GetComponent<LocalizeStringEvent>();
        }

        localizedNameString.StringReference.TableEntryReference = productSO.LocalizationKey;
        localizedTypeString.StringReference.TableEntryReference = $"Type{productSO.productType}";
        localizedSizeString.StringReference.TableEntryReference = $"Size{productSO.productSize}";

        nameValueText.text = localizedNameString.StringReference.GetLocalizedString();
        typeValueText.text = localizedTypeString.StringReference.GetLocalizedString();
        sizeValueText.text = localizedSizeString.StringReference.GetLocalizedString();
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
