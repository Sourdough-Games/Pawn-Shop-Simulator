using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerObjectHolder : Singleton<PlayerObjectHolder>
{
    [SerializeField] private FirstPersonController player;
    [SerializeField] private Transform holdPosition;
    [SerializeField] private ProductWorldCanvas screenSpaceCanvas;

    [SerializeField] private AudioSource pickupSound;

    public float reachDistance;

    private int holdLayer;

    private IHoldable heldObject;

    private Coroutine ReleaseZoomLockRoutine;

    private Transform HeldTransform {
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
        holdLayer = LayerMask.NameToLayer("HoldLayer");
    }

    void Update() {
        if(heldObject != null) {
            if(Input.GetKeyDown(KeyCode.X)) {
                DropHoldable();
            }
            if(Input.GetKeyDown(KeyCode.Mouse1)) {
                ThrowHoldable();
            }
        }
    }

    public bool TryPickupHoldable(IHoldable holdable)
    {
        if (heldObject != null || holdable == null || Singleton<PlayerController>.Instance.openModal != null || Singleton<PlayerController>.Instance.inVehicle != null) return false;

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

        player.enableZoom = false;

        heldObject = holdable;

        ProductWorldSlot currentSlot;
        if (HeldTransform.parent != null && HeldTransform.parent.TryGetComponent<ProductWorldSlot>(out currentSlot)) {
            currentSlot.ProductInSlot.ToggleHighlight(false);
            currentSlot.ProductInSlot = null;
            currentSlot.currentlySetPrice = 0;
        }

        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();

        //HeldTransform.GetComponentsInChildren<Collider>().All(c => c.enabled = false);
        
        rb.isKinematic = false;
        rb.useGravity = false;

        HeldTransform.SetParent(holdPosition);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        //Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponent<Collider>(), true);

        GenericPositionData HandPosition = holdable.GetHandPositionData();

        HeldTransform.localPosition = HandPosition.Position;
        HeldTransform.localRotation = Quaternion.Euler(HandPosition.Rotation);

        pickupSound.Play();

        Helper.SetLayerRecursively(HeldTransform.gameObject, holdLayer);

        if(holdable is Product) {
            screenSpaceCanvas.Setup(holdable as Product);
            screenSpaceCanvas.Show();
        }
    }

    void ThrowHoldable(float force = 15f, float multiplier = 10)
    {
        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();
        if(TryDropHoldable()) {
            //Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponent<Collider>(), true);

            rb.AddForce(transform.forward * force * multiplier);

            //Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponent<Collider>(), false);
        }
    }

    public bool TryDropHoldable(bool do_drop_validation = true) {
        if (heldObject == null || do_drop_validation && !CanDropHeldObject())
        {
            Debug.LogError("Cannot drop item");
            return false;
        }

        DropHoldable();
        return true;
    }

    private void DropHoldable()
    {
        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();

        //rb.isKinematic = false;
        rb.useGravity = true;

        HeldTransform.SetParent(null);

        Helper.SetLayerRecursively(HeldTransform.gameObject, 0);

        heldObject.Drop();

        //HeldTransform.GetComponentsInChildren<Collider>().All(c => c.enabled = true);
        rb.constraints = RigidbodyConstraints.None;
        //Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponent<Collider>(), false);

        heldObject = null;

        ReleaseZoomLockRoutine = StartCoroutine(ReleaseZoomLock());

        screenSpaceCanvas.Hide();
    }

    bool CanDropHeldObject()
    {
        return CurrentHoldable.CanBeDropped();
        // // Define the direction in which to cast the ray (forward from the player)
        // Vector3 forwardDirection = transform.forward;

        // // Define the maximum distance for the raycast
        // float maxDistance = 1.5f; // You can adjust this distance according to your needs

        // // Perform the raycast
        // RaycastHit hit;
        // bool isHit = Physics.Raycast(transform.position, forwardDirection, out hit, maxDistance);

        // return !isHit;
    }

    public IEnumerator ReleaseZoomLock() {
        yield return new WaitForSecondsRealtime(1f);
        player.enableZoom = true;
    }
}