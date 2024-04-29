using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductPriceTag : MonoBehaviour, IInteractable
{
    private ProductWorldSlot slot;
    private Outline ot;
    [SerializeField] private TextMeshProUGUI priceText;

    public bool reserved = false;

    //[SerializeField] private 
    void Start()
    {
        slot = GetComponentInParent<ProductWorldSlot>();
        ot = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if(slot.ProductInSlot == null) {
            ot.enabled = false;
            priceText.text = "";
            return;
        }

        if(reserved) {
            priceText.text = "RESERVED";
            return;
        }

        priceText.text = slot.currentlySetPrice == 0 ? "???" : Helper.ConvertToDollarAmount(slot.currentlySetPrice);
    }

    public void OpenPriceTagUI() {
        Singleton<GameManager>.Instance.configureDisplaySlotUI.Setup(slot);
    }

    public bool CanInteract()
    {
        return slot.ProductInSlot != null && !slot.IsReserved();
    }
}
