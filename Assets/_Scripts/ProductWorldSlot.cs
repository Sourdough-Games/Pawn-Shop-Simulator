using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductWorldSlot : MonoBehaviour
{

    private Outline outline;

    public Product ProductInSlot;

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

    public void Update() {

    }

    public bool TryInsertProduct(Product product) {
        if(product == null) return false;
        if(ProductInSlot != null) {
            return false;
        }
        if(!allowedSizes.Contains(product.ProductData.productSize)) {
            return false;
        }
        if(!product.ProductData.CanBeSlottedVertically && IsVertical) {
            return false;
        }

        InsertProduct(product);
        return true;
    }

    public void InsertProduct(Product product) {
        ProductInSlot = product;

        Singleton<PlayerObjectHolder>.Instance.DropHoldable();

        Transform p_transform = product.transform;

        p_transform.SetParent(this.transform);

        Rigidbody rb = p_transform.GetComponent<Rigidbody>();
        
        p_transform.GetComponentsInChildren<Collider>().All(c => c.enabled = false);

        rb.isKinematic = true;
        rb.useGravity = false;

        p_transform.localPosition = product.ProductData.WorldSlotPositionData.Position;
        p_transform.localRotation = Quaternion.Euler(product.ProductData.WorldSlotPositionData.Rotation);
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

    public bool CanTakeProduct
    {
        get
        {
            PlayerObjectHolder holder = Singleton<PlayerObjectHolder>.Instance;
            return holder != null && ProductInSlot != null && holder.CurrentHoldable == null;
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

        if(IsValidProductPlacement) {
            outline.enabled = true;
        } else if (CanTakeProduct) {
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

        if (IsValidProductPlacement) {
            if(holder.CurrentHoldable.PlaceIntoWorldSlot(this)) {
                ProductInSlot.ToggleHighlight(true);
                outline.enabled = false;
            }
        } else if (CanTakeProduct)
        {
            if (holder.TryPickupHoldable(ProductInSlot))
            {
                ProductInSlot.ToggleHighlight(false);
                ProductInSlot = null;
            }
        }
    }
}
