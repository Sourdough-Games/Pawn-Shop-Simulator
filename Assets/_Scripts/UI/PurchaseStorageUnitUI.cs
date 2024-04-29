using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PurchaseStorageUnitUI : Modal
{
    [SerializeField] private TextMeshProUGUI ownedStoragesText;
    [SerializeField] private StorageUnit[] PurchasableUnits;
    [SerializeField] private GameObject purchaseCard;
    [SerializeField] private Transform cardParent;

    [SerializeField] private StorageUnitsManager unitManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerMoneyManager moneyManager;

    bool generated = false;

    private float maxUnits = 3;

    public override void Draw() {
        StorageUnitsManager s = Singleton<StorageUnitsManager>.Instance;

        ownedStoragesText.text = $"{s.StorageUnits.Where(u => u.IsOwned).Count()}/{maxUnits}";
    }

    void Start() {
        unitManager = Singleton<StorageUnitsManager>.Instance;
        player = Singleton<PlayerController>.Instance;
    }

    new public void Open() {
        if(!generated) {
            generated = true;
            GenerateUI();
        }
        base.Open();
    }

    public void GenerateUI() {
        foreach(Transform t in cardParent.GetComponentInChildren<Transform>()) {
            Destroy(t.gameObject);
        }

        int loop = 1;
        foreach(StorageUnit unit in PurchasableUnits) {
            var obj = Instantiate(purchaseCard, cardParent);

            unit.currentPrice = Random.Range(125, 1750);

            var card = obj.GetComponent<PurchaseStorageUnitCard>();
            card.Setup(this, loop, unit.currentPrice);
            loop++;
        }
    }

    public bool TryPurchaseStorageUnit(int unit_id) {
        if (unitManager.StorageUnits.Count() < unit_id) {
            return false;
        }

        if (player.ownedStorageUnits.Count > maxUnits)
        {
            Singleton<NotificationSystem>.Instance.ShowMessage("MaxStorageUnitsReached");
            return false;
        }

        StorageUnit unit = unitManager.StorageUnits[unit_id];
        if(unit.IsOwned) {
            return false;
        }

        Debug.LogError($"Can Afford: {unit.currentPrice}");
        if(!moneyManager.CanTakeMoney(unit.currentPrice)) {
            Singleton<NotificationSystem>.Instance.ShowMessage("CannotAffordPurchase");
            return false;
        }

        PurchaseStorageUnit(unit);
        return true;
    }

    public void PurchaseStorageUnit(StorageUnit unit) {
        player.ownedStorageUnits.Add(unit);
        unit.IsOwned = true;

        moneyManager.TakeMoney(unit.currentPrice);
    }
}
