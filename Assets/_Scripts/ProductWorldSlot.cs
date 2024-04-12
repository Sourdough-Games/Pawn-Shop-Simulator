using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProductWorldSlot : MonoBehaviour
{

    private Outline outline;

    [SerializeField] private AudioSource placeProductSound;

    public Product ProductInSlot;

    public float currentlySetPrice = 0;

    public float maxDistance {
        get {
            return Singleton<PlayerObjectHolder>.Instance.reachDistance;
        }
    }

    [SerializeField] private ProductSize[] allowedSizes;

    [SerializeField] private bool IsVertical = false;

    void Start()
    {
        outline = GetComponent<Outline>();
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

        InsertProduct(product);
        return true;
    }

    public void InsertProduct(Product product) {
        ProductInSlot = product;

        Singleton<PlayerObjectHolder>.Instance.DropHoldable(false);

        Transform p_transform = product.transform;

        p_transform.SetParent(this.transform);

        Rigidbody rb = p_transform.GetComponent<Rigidbody>();
        
        //p_transform.GetComponentsInChildren<Collider>().All(c => c.enabled = false);

        rb.isKinematic = true;
        rb.useGravity = false;

        p_transform.localPosition = product.ProductData.WorldSlotPositionData.Position;
        p_transform.localRotation = Quaternion.Euler(product.ProductData.WorldSlotPositionData.Rotation);

        placeProductSound.Play();
    }

    public bool IsValidProductPlacement {
        get {
            PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

            if(holder == null || holder.CurrentHoldable == null) return false;

            if(!holder.CurrentHoldable.CanBeSlottedVertically() && IsVertical) {
                return false;
            }

            return ProductInSlot == null && 
                   allowedSizes.Contains(holder.CurrentHoldable.GetProductSize());
        }
    }

    bool PlayerIsWithinDistance
    {
        get
        {
            float distance = Vector3.Distance(transform.position, Singleton<PlayerObjectHolder>.Instance.transform.position);
            return distance <= maxDistance;
        }
    }

    void OnMouseOver()
    {
        if(!PlayerIsWithinDistance) return;

        if(ProductInSlot == null) {
            outline.enabled = true;
        } else {
            ProductInSlot.ToggleHighlight(true);
        }
    }

    void OnMouseExit()
    {
        outline.enabled = false;

        if(ProductInSlot != null) {
            ProductInSlot.ToggleHighlight(false);
        }
    }

    void OnMouseDown()
    {
        if (!PlayerIsWithinDistance) return;

        PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;

        if (ProductInSlot == null && holder.CurrentHoldable != null) {
            if(holder.CurrentHoldable.PlaceIntoWorldSlot(this)) {
                ProductInSlot.ToggleHighlight(true);
                outline.enabled = false;
            }
        } else {
            if (holder.TryPickupHoldable(ProductInSlot))
            {
                ProductInSlot.ToggleHighlight(false);
                ProductInSlot = null;
            }
        }
    }
}
