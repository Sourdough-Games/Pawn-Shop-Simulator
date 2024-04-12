using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductPriceTag : MonoBehaviour
{
    private ProductWorldSlot slot;
    private Outline ot;
    [SerializeField] private TextMeshProUGUI priceText;

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

        priceText.text = slot.currentlySetPrice == 0 ? "???" : Helper.ConvertToDollarAmount(slot.currentlySetPrice);

        if(!Helper.IsWithinPlayerReach(transform)) {
            ot.enabled = false;
        }
    }

    public void OnMouseOver() {
        if(Helper.IsWithinPlayerReach(transform) && slot.ProductInSlot != null) {
            ot.enabled = true;
        }
    }

    public void OnMouseExit() {
        ot.enabled = false;
    }

    public void OnMouseDown() {
        if(Helper.IsWithinPlayerReach(transform) && slot.ProductInSlot != null) {
            Singleton<GameManager>.Instance.configureDisplaySlotUI.Setup(slot);
        }
    }
}
