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
        if (heldObject != null) return false;

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

        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();

        HeldTransform.GetComponentsInChildren<Collider>().All(c => c.enabled = false);
        
        rb.isKinematic = true;
        rb.useGravity = false;

        HeldTransform.SetParent(holdPosition);

        ProductPositionData HandPosition = holdable.GetHandPositionData();

        HeldTransform.localPosition = HandPosition.Position;
        HeldTransform.localRotation = Quaternion.Euler(HandPosition.Rotation);

        pickupSound.Play();

        Helper.SetLayerRecursively(HeldTransform.gameObject, holdLayer);

        if(holdable is Product) {
            screenSpaceCanvas.Setup((holdable as Product).ProductData);
            screenSpaceCanvas.Show();
        }
    }

    void ThrowHoldable(float force = 15f, float multiplier = 10)
    {
        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();
        DropHoldable();
        
        Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponentInChildren<Collider>(), true);

        rb.AddForce(transform.forward * force * multiplier);

        Physics.IgnoreCollision(rb.GetComponentInChildren<Collider>(), GetComponentInChildren<Collider>(), false);
    }

    public void DropHoldable()
    {
        if(heldObject == null) return;

        StopClipping();

        Rigidbody rb = HeldTransform.GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        HeldTransform.SetParent(null);

        Helper.SetLayerRecursively(HeldTransform.gameObject, 0);

        heldObject.Drop();

        HeldTransform.GetComponentsInChildren<Collider>().All(c => c.enabled = true);

        heldObject = null;

        ReleaseZoomLockRoutine = StartCoroutine(ReleaseZoomLock());

        screenSpaceCanvas.Hide();
    }

    void StopClipping() //function only called when dropping/throwing
    {
        Vector3 directionToHeldObject = HeldTransform.position - transform.position;
        float clipRange = directionToHeldObject.magnitude;

        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, directionToHeldObject.normalized, out hit, clipRange, ~9);
        if (isHit)
        {

            Debug.LogError($"{hit.collider.gameObject.name}");

            //change object position to camera position 
            HeldTransform.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    public IEnumerator ReleaseZoomLock() {
        yield return new WaitForSecondsRealtime(1f);
        player.enableZoom = true;
    }
}