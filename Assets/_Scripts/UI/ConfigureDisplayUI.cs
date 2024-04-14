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

        priceInput.ActivateInputField();
        priceInput.onEndEdit.AddListener(OnEndEdit);
    }

    public void SetPrice() {
        productSlot.currentlySetPrice = float.Parse(priceInput.text.Trim());
        Close();
    }

    private void OnEndEdit(string value)
    {
        // Check if the pressed key was Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Call the method to set the price
            SetPrice();
        }
    }

    public override void Draw()
    {
        
    }
}
