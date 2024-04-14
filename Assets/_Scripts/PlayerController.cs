using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private EscapeMenuUI escapeMenu;

    [SerializeField] private float storageUnitProductSpawnMin = 1f;
    [SerializeField] private float storageUnitProductSpawnMax = 15f;

    public List<StorageUnit> ownedStorageUnits;

    public CarController inVehicle;

    public Modal openModal;

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

    public bool TryEnterVehicle(CarController vehicle) {
        if(inVehicle != null) return false;

        EnterVehicle(vehicle);
        return true;
    }

    private void EnterVehicle(CarController vehicle) {
        
        Singleton<PlayerObjectHolder>.Instance.enabled = false;

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

        Singleton<PlayerObjectHolder>.Instance.enabled = true;
    }

    void Update() {
        if(openModal == null && Input.GetKeyDown(KeyCode.Escape)) {
            escapeMenu.Open();
        }
    }
}
