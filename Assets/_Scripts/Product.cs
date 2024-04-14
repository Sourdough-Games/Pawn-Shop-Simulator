using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour, IHoldable
{
    [SerializeField] private ProductWorldCanvas LookAtProductCanvas;
    [SerializeField] private ProductSO Data;
    [SerializeField] private Collider denyDropCollider;

    public float currentlySetPrice = 0;

    public float maxDistance {
        get {
            return Singleton<PlayerObjectHolder>.Instance.reachDistance;
        }
    }

    public ProductSO ProductData
    {
        get
        {
            return Data;
        }
    }

    private Outline highlight;

    private bool isHeld;

    public bool IsHeld {
        get {
            return isHeld;
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

    public bool isColliding = false;

    [SerializeField] private TruckBed inTruckBed;

    bool PlayerIsWithinDistance
    {
        get {
            float distance = Vector3.Distance(transform.position, Singleton<PlayerObjectHolder>.Instance.transform.position);

            // Check if the distance is within the allowed range
            return distance <= maxDistance;
        }
    }

    void Start() {
        highlight = GetComponent<Outline>();
        ToggleHighlight(false);

        FaceTarget ft = LookAtProductCanvas.gameObject.AddComponent<FaceTarget>();
        ft.target = Singleton<PlayerObjectHolder>.Instance.transform;
        LookAtProductCanvas.Hide();
        LookAtProductCanvas.Setup(this);

        denyDropCollider.enabled = false;
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

    public void Drop()
    {
        isHeld = false;
        denyDropCollider.enabled = false;
    }

    public void PickUp()
    {
        var c = Singleton<PlayerController>.Instance;

        if (c.openModal == null && c.inVehicle == null && Singleton<PlayerObjectHolder>.Instance.TryPickupHoldable(this)) {
            isHeld = true;
            denyDropCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        TruckBed truckBed;
        if(other.TryGetComponent<TruckBed>(out truckBed)) {
            inTruckBed = truckBed;
        }
    }

    private void OnTriggerStay() {
        if(isHeld) {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        isColliding = false;
    }

    void OnMouseDown() {
        if(!PlayerIsWithinDistance) return;

        PickUp();
    }

    void OnMouseOver() {
        if (!PlayerIsWithinDistance && Singleton<PlayerController>.Instance.inVehicle != null && Singleton<PlayerController>.Instance.openModal != null) return;

        ToggleHighlight(true);
    }

    void OnMouseExit() {
        ToggleHighlight(false);
    }

    public bool PlaceIntoWorldSlot(ProductWorldSlot slot)
    {
        if(slot.TryInsertProduct(this)) {
            isHeld = false;
            isInSlot = true;

            return true;
        }

        return false;
    }

    public void ToggleHighlight(bool on)
    {
        if (on)
        {
            LookAtProductCanvas.Show();
        }
        else
        {
            LookAtProductCanvas.Hide();
        }
        highlight.enabled = on;
    }

    public ProductSize GetProductSize()
    {
        return ProductData.productSize;
    }

    public GenericPositionData GetSlotPositionData()
    {
        return Data.WorldSlotPositionData;
    }

    public GenericPositionData GetHandPositionData()
    {
        return Data.HandSlotPositionData;
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
        Debug.LogError(isColliding);
        return !isColliding;
    }
}
