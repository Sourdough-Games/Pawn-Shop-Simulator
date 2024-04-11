using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PurchaseStorageUnitUI : Modal
{
    [SerializeField] private TextMeshProUGUI ownedStoragesText;



    public override void Draw() {
        StorageUnitsManager s = Singleton<StorageUnitsManager>.Instance;

        ownedStoragesText.text = $"Owned Units: {s.StorageUnits.Where(u => u.IsOwned).Count()}/1";
    }

    public void PurchaseStorageUnit() {
        StorageUnitsManager s = Singleton<StorageUnitsManager>.Instance;
        PlayerController c = Singleton<PlayerController>.Instance;

        if(c.ownedStorageUnits.Count > 0) {
            Debug.LogError("Can only purchase 1 at a time");
            return;
        }

        StorageUnit random_unit = s.StorageUnits.Where(u => !u.IsOwned).OrderBy(u => Guid.NewGuid()).FirstOrDefault();
        if(random_unit != null) {
            c.ownedStorageUnits.Add(random_unit);

            random_unit.IsOwned = true;
        }
    }
}
