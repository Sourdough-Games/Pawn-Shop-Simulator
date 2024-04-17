using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour, IHoldable
{
    [SerializeField] private ProductWorldCanvas LookAtProductCanvas;
    [SerializeField] private ProductSO Data;

    public float currentlySetPrice = 0;

    public ProductSO ProductData
    {
        get
        {
            return Data;
        }
    }

    private bool isInSlot;

    public bool IsInSlot
    {
        get
        {
            return isInSlot;
        }
    }

    public ProductWorldSlot onProductSlotTrigger;

    [SerializeField] private TruckBed inTruckBed;

    private PlayerController controller;

    void Start() {

        FaceTarget ft = LookAtProductCanvas.gameObject.AddComponent<FaceTarget>();
        ft.target = Singleton<PlayerController>.Instance.transform;
        LookAtProductCanvas.Hide();
        LookAtProductCanvas.Setup(this);
    }

    void Update() {
        if(inTruckBed != null) {

            Rigidbody rb = GetComponent<Rigidbody>();

            if(inTruckBed.IsTruckRunning) {
                rb.isKinematic = true;
                rb.excludeLayers =  LayerMask.GetMask("Vehicle", "Ignore Raycast");
                transform.SetParent(inTruckBed.transform);
            } else {
                rb.isKinematic = false;
                transform.SetParent(null);
                rb.excludeLayers = LayerMask.GetMask();

                inTruckBed.bedCollider.enabled = false;

                inTruckBed = null;
            }
        }
    }

    public bool PlaceIntoWorldSlot(ProductWorldSlot slot)
    {
        if(slot.TryInsertProduct(this)) {
            isInSlot = true;

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
        if (other.TryGetComponent<TruckBed>(out truckBed))
        {
            inTruckBed = truckBed;
        }
    }

    private void OnTriggerExit(Collider other) {
        onProductSlotTrigger = null;
        inTruckBed = null;
    }

    private void OnMouseDown() {
        Singleton<PlayerObjectHolder>.Instance.TryPickupHoldable(this);
    }
}
