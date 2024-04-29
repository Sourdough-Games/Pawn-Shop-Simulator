using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour, IHoldable, IInteractable
{
    [SerializeField] private ProductWorldCanvas LookAtProductCanvas;
    [SerializeField] private ProductSO Data;

    public float currentlySetPrice = 0;

    private Rigidbody rb;

    public ProductSO ProductData
    {
        get
        {
            return Data;
        }
    }

    public ProductWorldSlot currentSlot;

    public bool IsInSlot
    {
        get
        {
            return currentSlot != null;
        }
    }

    public ProductWorldSlot onProductSlotTrigger;

    [SerializeField] private TruckBed inTruckBed;

    public TruckBed TruckBed {
        get {
            return inTruckBed;
        }
        set {
            if(value == null && inTruckBed != null) {
                inTruckBed.TruckBedChanged -= HandleTruckBedChanged;
            }
            inTruckBed = value;
        }
    }

    private PlayerController controller;

    void Start() {

        FaceTarget ft = LookAtProductCanvas.gameObject.AddComponent<FaceTarget>();
        ft.target = Singleton<PlayerController>.Instance.transform;
        LookAtProductCanvas.Hide();
        LookAtProductCanvas.Setup(this);

        rb = GetComponent<Rigidbody>();
    }

    public bool PlaceIntoWorldSlot(ProductWorldSlot slot)
    {
        if(slot.TryInsertProduct(this)) {
            currentSlot = slot;
            return true;
        }

        return false;
    }

    public ProductSize GetProductSize()
    {
        return ProductData.productSize;
    }

    public GenericPositionData GetSlotPositionData()
    {
        return Data.WorldSlotPositionData;
    }

    public bool CanBeSlottedVertically()
    {
        return Data.CanBeSlottedVertically;
    }

    public void ToggleWorldspaceUI(bool on)
    {
        LookAtProductCanvas.gameObject.SetActive(on);
    }

    public bool CanBeDropped()
    {
        return true;
    }

    private void OnTriggerStay(Collider other) {
        if(onProductSlotTrigger != null) return;

        ProductWorldSlot slot;
        if (other.TryGetComponent<ProductWorldSlot>(out slot))
        {
            onProductSlotTrigger = slot;
        }

        TruckBed truckBed;
        if (other.TryGetComponent<TruckBed>(out truckBed) && TruckBed == null)
        {
            truckBed.TruckBedChanged += HandleTruckBedChanged;
            TruckBed = truckBed;
        }
    }

    private void HandleTruckBedChanged(bool is_locked)
    {
        if (is_locked) {
            rb.isKinematic = true;
            rb.excludeLayers = LayerMask.GetMask("Vehicle", "Ignore Raycast");
            transform.SetParent(TruckBed.transform);
            return;
        }

        rb.isKinematic = false;
        transform.SetParent(null);
        rb.excludeLayers = LayerMask.GetMask();

        TruckBed = null;
    }

    private void OnTriggerExit(Collider other) {
        onProductSlotTrigger = null;
        inTruckBed = null;
    }

    private void OnMouseDown() {
        if(onProductSlotTrigger != null) {
            if(onProductSlotTrigger.IsReserved()) return;
        }
        Singleton<PlayerObjectHolder>.Instance.TryPickupHoldable(this);
    }

    public bool CanInteract()
    {
        return true;
    }
}
