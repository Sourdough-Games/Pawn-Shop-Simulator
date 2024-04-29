using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureDisplaySlotUI : Modal
{
    [SerializeField] private TMP_InputField priceInput;
    [SerializeField] private Button setLastButton;

    public ProductWorldSlot productSlot;

    public float lastSetPrice;

    public void Setup(ProductWorldSlot slot) {
        productSlot = slot;

        priceInput.text = slot.currentlySetPrice.ToString();

        Open();

        priceInput.ActivateInputField();
        priceInput.onEndEdit.AddListener(OnEndEdit);
    }

    public void SetPrice() {
        var new_price = float.Parse(priceInput.text.Trim());
        if (new_price > 0)
        {
            lastSetPrice = new_price;
        }

        productSlot.currentlySetPrice = new_price;
        Close();
    }

    public void SetLastPrice()
    {
        priceInput.text = lastSetPrice.ToString();
        SetPrice();
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
        if(lastSetPrice == 0) {
            setLastButton.gameObject.SetActive(false);
        } else {
            setLastButton.gameObject.SetActive(true);
        }
    }
}
