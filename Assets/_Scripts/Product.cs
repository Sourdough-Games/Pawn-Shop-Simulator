using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Product : MonoBehaviour, IHoldable
{
    [SerializeField] private ProductWorldCanvas LookAtProductCanvas;
    [SerializeField] private ProductSO Data;

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
        LookAtProductCanvas.Setup(Data);
    }

    public void Drop()
    {
        isHeld = false;
    }

    public void PickUp()
    {
        if (Singleton<PlayerController>.Instance.openModal == null && Singleton<PlayerObjectHolder>.Instance.TryPickupHoldable(this)) {
            isHeld = true;
        }
    }

    void OnMouseDown() {
        if(!PlayerIsWithinDistance) return;

        PickUp();
    }

    void OnMouseOver() {
        if (!PlayerIsWithinDistance) return;

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

    public ProductPositionData GetSlotPositionData()
    {
        return Data.WorldSlotPositionData;
    }

    public ProductPositionData GetHandPositionData()
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
}
