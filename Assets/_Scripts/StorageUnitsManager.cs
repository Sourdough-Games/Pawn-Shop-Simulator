using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUnitsManager : Singleton<StorageUnitsManager>
{
    public StorageUnit[] StorageUnits;

    void Start() {
        StorageUnits = GetComponentsInChildren<StorageUnit>();
        CloseAllUnits();
    }

    void CloseAllUnits() {
        Debug.LogError("CloseAllUnits");
        foreach (StorageUnit unit in StorageUnits) {
            unit.CloseUnit(false);
            unit.CanSpawn = true;
        }
    }
}
