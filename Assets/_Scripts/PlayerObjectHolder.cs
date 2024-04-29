using System.Net.Http;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerObjectHolder : Singleton<PlayerObjectHolder>
{
    private PlayerController controller;
    [SerializeField] private Transform holdPosition;
    [SerializeField] private ProductWorldCanvas screenSpaceCanvas;

    [SerializeField] private AudioSource pickupSound;

    [SerializeField] private float pickupForce = 5f;
    [SerializeField] private float rotationForce = 5f;

    private IHoldable heldObject;
    private Rigidbody heldRB;
    float heldRbOriginalMass;

    private Coroutine ReleaseZoomLockRoutine;

    private bool isRotating;
    private Vector3 lastMousePosition;

    public Transform HeldTransform {
        get {
            return (heldObject as MonoBehaviour).transform;
        }
    }

    public IHoldable CurrentHoldable {
        get {
            return heldObject;
        }
    }

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    private Coroutine rotatingCoroutine;


    void Update()
    {
        if (heldObject != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, HeldTransform.position);

            if (distanceToPlayer > controller.Reach || Input.GetKeyDown(KeyCode.X) && !isRotating)
            {
                DropHoldable();
                return;
            }

            Product product;
            if(Input.GetKeyDown(KeyCode.Mouse0) && HeldTransform.TryGetComponent<Product>(out product)) {
                if(product.onProductSlotTrigger == null) return;

                if(product.onProductSlotTrigger.TryInsertProduct(product)) {
                    return;
                }
            }

            if(isRotating = Input.GetMouseButton(1)) {
                if(rotatingCoroutine == null) {
                    rotatingCoroutine = StartCoroutine(HandleRotationUnfreeze());
                }
                lastMousePosition = Input.mousePosition;
            }

            float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheelInput != 0)
            {
                // Calculate the new local position after scrolling
                float newPosition = holdPosition.localPosition.z + scrollWheelInput;

                float clamp = Math.Clamp(newPosition, 1, controller.Reach - 1);

                // Set the new position while maintaining the same direction from the player
                holdPosition.localPosition = new Vector3(0, 0, clamp);
            }
        }
    }

    private IEnumerator HandleRotationUnfreeze()
    {
        controller.FPC.cameraCanMove = false;

        while(isRotating) {
            yield return new WaitForSeconds(.01f);
        }
        controller.FPC.cameraCanMove = true;

        rotatingCoroutine = null;
    }

    void FixedUpdate() {
        if(heldObject != null) {
            if (isRotating)
            {
                RotateHoldPosition();
            }

            MoveObject();
        }
    }

    void MoveObject()
    {
        // Calculate the center of mass position
        Vector3 centerOfMass = heldRB.worldCenterOfMass;

        // Calculate the distance between the center of mass and the hold position
        float distance = Vector3.Distance(centerOfMass, holdPosition.position);

        if (distance > 0.1f)
        {
            // Calculate the move direction based on the center of mass
            Vector3 moveDirection = holdPosition.position - centerOfMass;

            // Align the held object with the hold position
            HeldTransform.rotation = holdPosition.rotation;

            // Apply force to move the object towards the hold position
            heldRB.AddForce(moveDirection.normalized * pickupForce * distance);
        }
    }
    void RotateHoldPosition()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationForce * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationForce * Time.fixedDeltaTime;

        Camera cam = controller.FPC.playerCamera;

        Vector3 right = Vector3.Cross(cam.transform.up, holdPosition.position - cam.transform.position);
        Vector3 up = Vector3.Cross(holdPosition.position - cam.transform.position, right);

        holdPosition.rotation = Quaternion.AngleAxis(-mouseX, up) * holdPosition.rotation;
        holdPosition.rotation = Quaternion.AngleAxis(mouseY, right) * holdPosition.rotation;

        // Update the held object's rotation
        HeldTransform.rotation = holdPosition.rotation;
    }

    public bool TryPickupHoldable(IHoldable holdable)
    {
        if (heldObject != null || holdable == null || controller.IsInteracting) return false;

        Product product;
        ProductWorldSlot slot = null;

        if ((holdable as MonoBehaviour).TryGetComponent<Product>(out product)) {
            if (product.transform.parent != null && product.transform.parent.TryGetComponent<ProductWorldSlot>(out slot))
            {
                if(slot.IsReserved()) {
                    Singleton<NotificationSystem>.Instance.ShowMessage("CantTakeFromReservedSlot");
                    return false;
                }
            }
        }

        holdable.ToggleWorldspaceUI(false);
        PickupHoldable(holdable);
        return true;
    }

    private void PickupHoldable(IHoldable holdable)
    {
        if(ReleaseZoomLockRoutine != null) {
            StopCoroutine(ReleaseZoomLockRoutine);
            ReleaseZoomLockRoutine = null;
        }

        controller.FPC.enableZoom = false;
        heldObject = holdable;

        HeldTransform.gameObject.layer = 9;
        heldRB = HeldTransform.GetComponent<Rigidbody>();

        heldRbOriginalMass = heldRB.mass;
        heldRB.mass = 1;
        heldRB.isKinematic = false;

        //heldRB.excludeLayers |= 1 << 8;

        heldRB.useGravity = false;
        heldRB.drag = 10;
        heldRB.constraints = RigidbodyConstraints.FreezeRotation;

        holdPosition.rotation = HeldTransform.rotation;

        pickupSound.Play();

        if(holdable is Product) {
            screenSpaceCanvas.Setup(holdable as Product);
            screenSpaceCanvas.Show();

            ProductWorldSlot currentSlot;
            if (HeldTransform.parent != null && HeldTransform.parent.TryGetComponent<ProductWorldSlot>(out currentSlot))
            {
                currentSlot.RemoveObject();
            }
        }

        HeldTransform.SetParent(null);
    }

    public bool TryDropHoldable() {
        if (heldObject == null || !CanDropHeldObject())
        {
            Debug.LogError("Cannot drop item");
            return false;
        }

        DropHoldable();
        return true;
    }

    private void DropHoldable()
    {
        //heldRB.excludeLayers &= ~(1 << 8);

        heldRB.mass = heldRbOriginalMass;

        heldRB.useGravity = true;
        heldRB.drag = 1;
        heldRB.constraints = RigidbodyConstraints.None;

        HeldTransform.gameObject.layer = 0;
        //HeldTransform.parent = null;

        heldObject = null;
        heldRB = null;

        ReleaseZoomLockRoutine = StartCoroutine(ReleaseZoomLock());

        screenSpaceCanvas.Hide();
    }

    bool CanDropHeldObject()
    {
        return CurrentHoldable.CanBeDropped();
    }

    public IEnumerator ReleaseZoomLock() {
        yield return new WaitForSecondsRealtime(1f);
        controller.FPC.enableZoom = true;
    }
}