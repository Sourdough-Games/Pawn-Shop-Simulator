using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProductWorldSlot : MonoBehaviour, IInteractable
{

    [SerializeField] private Collider _collider;

    private Outline outline;

    private ProductPriceTag priceTag;

    [SerializeField] private AudioSource placeProductSound;
    [SerializeField] public Transform customerStandPosition;

    public Product ProductInSlot;

    public bool isSelectedByNPC = false;

    public float currentlySetPrice {
        get {
            return ProductInSlot != null ? ProductInSlot.currentlySetPrice : 0;
        }
        set {
            if(ProductInSlot != null) {
                ProductInSlot.currentlySetPrice = value;
            }
        }
    }
    
    public ProductPriceTag PriceTag {
        get {
            return priceTag;
        }
    }

    [SerializeField] private ProductSize[] allowedSizes;

    [SerializeField] private bool IsVertical = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        priceTag = GetComponentInChildren<ProductPriceTag>();
    }

    private void OnTriggerStay(Collider other) {
        Product product;
        if(other.TryGetComponent<Product>(out product) && product.onProductSlotTrigger == this) {
            outline.enabled = true;
        }  
    }

    private void OnTriggerExit() {
        outline.enabled = false;
    }

    public bool TryInsertProduct(Product product) {
        if(product == null) return false;
        if(ProductInSlot != null) {
            Singleton<NotificationSystem>.Instance.ShowMessage("ProductAlreadyInSlot");
            return false;
        }
        if(!allowedSizes.Contains(product.ProductData.productSize)) {
            Singleton<NotificationSystem>.Instance.ShowMessage("ProductIncorrectSlotSize");
            return false;
        }
        if(!product.ProductData.CanBeSlottedVertically && IsVertical) {
            Singleton<NotificationSystem>.Instance.ShowMessage("ProductCantBeVertical");
            return false;
        }
        if(!IsValidProductPlacement) {
            return false;
        }

        InsertProduct(product);
        return true;
    }

    public void InsertProduct(Product product) {
        ProductInSlot = product;

        Singleton<PlayerObjectHolder>.Instance.TryDropHoldable();

        Transform p_transform = product.transform;

        p_transform.SetParent(this.transform);

        Rigidbody rb = p_transform.GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        p_transform.localPosition = product.ProductData.WorldSlotPositionData.Position;
        p_transform.localRotation = Quaternion.Euler(product.ProductData.WorldSlotPositionData.Rotation);

        placeProductSound.Play();
    }

    public void MarkReserved() {
        PriceTag.reserved = true;
    }

    public bool IsReserved() {
        return PriceTag.reserved;
    }

    internal void Unreserve()
    {
        PriceTag.reserved = false;
    }

    internal void RemoveObject()
    {
        if(ProductInSlot == null) return;
        
        ProductInSlot.transform.SetParent(null);
        
        ProductInSlot = null;
        currentlySetPrice = 0;
        priceTag.reserved = false;
    }

    public bool CanInteract()
    {
        return IsValidProductPlacement;
    }

    public bool IsValidProductPlacement {
        get {
            PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

            if(holder == null || holder.CurrentHoldable == null || IsReserved()) return false;

            if(!holder.CurrentHoldable.CanBeSlottedVertically() && IsVertical) {
                return false;
            }

            return ProductInSlot == null && 
                   allowedSizes.Contains(holder.CurrentHoldable.GetProductSize());
        }
    }

    // void OnMouseDown()
    // {
    //     PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

    //     if (ProductInSlot == null && holder.CurrentHoldable != null) {
    //         if(holder.CurrentHoldable.PlaceIntoWorldSlot(this)) {
    //             outline.enabled = false;
    //         }
    //     }
    // }
}
