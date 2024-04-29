using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PurchaseStorageUnitCard : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent unitNameString;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button purchaseButton;

    private PurchaseStorageUnitUI ui;

    private int unitNumber = 0;
    private int UnitID {
        get {
            return unitNumber - 1;
        }
    }

    public int UnitNumber {
        get {
            return unitNumber;
        } set {
            unitNumber = value;
        }
    }

    void Start() {
        purchaseButton.onClick.AddListener(HandlePurchaseButtonClicked);
    }

    public void Setup(PurchaseStorageUnitUI ui, int unit_number, float price) {
        this.ui = ui;

        unitNumber = unit_number;

        (unitNameString.StringReference["Number"] as IntVariable).Value = unit_number;
        unitNameString.StringReference.RefreshString();

        costText.text = Helper.ConvertToDollarAmountNoCollapse(price);
    }

    private void HandlePurchaseButtonClicked()
    {
        if(ui != null && ui.TryPurchaseStorageUnit(UnitID)) {
            gameObject.SetActive(false);
        }
    }
}
