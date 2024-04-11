using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private EscapeMenuUI escapeMenu;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private float storageUnitProductSpawnMin = 1f;
    [SerializeField] private float storageUnitProductSpawnMax = 15f;

    public List<StorageUnit> ownedStorageUnits;

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

    void Update() {
        if(openModal == null && Input.GetKeyDown(KeyCode.Escape)) {
            escapeMenu.Open();
        }
    }
}
