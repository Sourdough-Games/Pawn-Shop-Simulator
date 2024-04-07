using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductWorldCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameValueText;
    [SerializeField] private TextMeshProUGUI typeValueText;
    [SerializeField] private TextMeshProUGUI sizeValueText;

    public bool CanShow = true;

    public void Setup(ProductSO productSO) {
        nameValueText.text = productSO.ProductName;
        typeValueText.text = productSO.productType.ToString();
        sizeValueText.text = productSO.productSize.ToString();
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
