using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfigureDisplaySlotUI : Modal
{
    [SerializeField] private TMP_InputField priceInput;

    public ProductWorldSlot productSlot;

    public void Setup(ProductWorldSlot slot) {
        productSlot = slot;

        priceInput.text = slot.currentlySetPrice.ToString();

        Open();
    }

    public void SetPrice() {
        productSlot.currentlySetPrice = float.Parse(priceInput.text.Trim());
        Close();
    }

    public override void Draw()
    {
        
    }
}
