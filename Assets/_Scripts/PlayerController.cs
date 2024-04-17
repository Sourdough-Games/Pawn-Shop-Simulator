using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private EscapeMenuUI escapeMenu;

    [SerializeField] private float storageUnitProductSpawnMin = 1f;
    [SerializeField] private float storageUnitProductSpawnMax = 15f;
    [SerializeField] private float reachDistance = 5f;

    private Rigidbody rigidBody;
    private FirstPersonController fpc;

    private PlayerObjectHolder holder;
    public List<StorageUnit> ownedStorageUnits;

    public CarController inVehicle;

    public Modal openModal;

    public float Reach {
        get {
            return reachDistance;
        }
    }

    public FirstPersonController FPC {
        get {
            return fpc;
        }
    }

    public Rigidbody RigidBody {
        get {
            return rigidBody;
        }
    }

    public float StorageUnitProductSpawnMin {
        get {
            return storageUnitProductSpawnMin;
        }
    }

    public float StorageUnitProductSpawnMax
    {
        get
        {
            return storageUnitProductSpawnMax;
        }
    }

    public bool IsInteracting {
        get {
            return !(inVehicle == null && openModal == null && fpc.playerCanMove && holder.CurrentHoldable == null);
        }
    }

    public bool CanInteractWithTransform(Transform transform, float additional_reach = 0f, bool additional_conditions = true) {
        return additional_conditions && !IsInteracting && Helper.IsWithinPlayerReach(transform, reachDistance + additional_reach);
    }

    new void Awake() {
        base.Awake();

        fpc = GetComponent<FirstPersonController>();
        rigidBody = GetComponent<Rigidbody>();
        holder = GetComponent<PlayerObjectHolder>();
    }

    public bool TryEnterVehicle(CarController vehicle) {
        if(inVehicle != null) return false;

        EnterVehicle(vehicle);
        return true;
    }

    private void EnterVehicle(CarController vehicle) {
        inVehicle = vehicle;
        vehicle.BeingOperated = true;

        transform.SetParent(vehicle.transform);
        transform.localPosition = vehicle.VehicleData.sitPosition.Position;
        transform.localRotation = Quaternion.Euler(vehicle.VehicleData.sitPosition.Rotation);

        Singleton<FirstPersonController>.Instance.Freeze();

        vehicle.Camera.gameObject.SetActive(true);
    }

    public bool TryExitVehicle() {
        Debug.Log("Attempt Exit");
        if (inVehicle == null || inVehicle.BeingOperated == false) return false;
        if(inVehicle.exitPosition.isColliding) {
            Debug.Log("Exit Position Is Colliding");
            return false;
        }

        ExitVehicle();
        return true;
    }

    private void ExitVehicle() {

        
        transform.SetParent(null);
        transform.position = inVehicle.exitPosition.transform.position;
        transform.rotation = inVehicle.exitPosition.transform.rotation;

        Singleton<FirstPersonController>.Instance.Unfreeze();

        inVehicle.Camera.gameObject.SetActive(false);

        inVehicle.BeingOperated = false;
        inVehicle = null;
    }

    void Update() {
        if(openModal == null && Input.GetKeyDown(KeyCode.Escape)) {
            escapeMenu.Open();
        }
    }
}
