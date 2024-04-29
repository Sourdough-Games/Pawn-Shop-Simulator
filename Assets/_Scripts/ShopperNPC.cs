using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ShopperState {
    None,
    BrowsingShop,
    ReadyToLeave,
    WaitingAtRegister,
    Leaving,
    WaitingToDespawn
}

public class ShopperNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private ShopperState _state = ShopperState.None;
    [SerializeField] private ShopManager shopManager;

    private Character character;

    private List<ProductWorldSlot> browsedSlots = new();
    private List<ProductWorldSlot> reservedSlots = new();

    private Coroutine activeCoroutine;

    private float originalMoney;
    public float currentMoney;

    bool isStandingInLine = false;

    private bool isSetup = false;

    private RegisterTable atRegisterTable;

    public ShopperState State {
        get { return _state; }
        set { 
            _state = value;
            if(activeCoroutine != null) {
                StopCoroutine(activeCoroutine);
            }
        }
    }

    public List<ProductWorldSlot> ReservedSlots {
        get {
            return reservedSlots;
        }
    }

    void Update() {
        if(!isSetup) return;

        switch(State) {
            case ShopperState.None:

                State = ShopperState.BrowsingShop;
                activeCoroutine = StartCoroutine(BrowseShop());
                
                break;
            case ShopperState.ReadyToLeave:
                if(reservedSlots.Count > 0) {
                    if(atRegisterTable == null) {
                        atRegisterTable = shopManager.RegisterTables[Random.Range(0, shopManager.RegisterTables.Count)];
                    }

                    if(atRegisterTable != null) {
                        atRegisterTable.AddShopper(this);
                        State = ShopperState.WaitingAtRegister;
                    }
                } else {
                    State = ShopperState.Leaving;
                }
                break;
            case ShopperState.WaitingAtRegister:
                if(isStandingInLine == false) {
                    atRegisterTable.LineChanged.AddListener(HandleStandingInLine);
                    HandleStandingInLine();
                    isStandingInLine = true;
                }
                break;
            case ShopperState.Leaving:
                if(activeCoroutine != null) {
                    StopCoroutine(activeCoroutine);
                }
                if(isStandingInLine) {
                    isStandingInLine = false;
                    atRegisterTable.LineChanged.RemoveListener(HandleStandingInLine);
                }
                State = ShopperState.WaitingToDespawn;
                StartCoroutine(HandleLeaving());
                break;
        }
    }

    private void HandleStandingInLine() {
        character.StopWalking();

        int position_in_line = atRegisterTable.GetShopperLinePosition(this);
        Transform position_transform = atRegisterTable.transform;
        Vector3 new_position = atRegisterTable.CustomerStandPosition.position;

        if (position_in_line > 0)
        {
            position_transform = atRegisterTable.GetShopperAtPosition(position_in_line - 1).transform;
            new_position = position_transform.position - position_transform.transform.forward * 1f;
        }
        
        character.SetNewDestination(position_transform.position, new_position);
    }

    void Start()
    {
        character = GetComponent<Character>();
    }

    public void Setup(ShopManager manager, float startingMoney) {
        shopManager = manager;
        isSetup = true;

        originalMoney = startingMoney;
        currentMoney = originalMoney;

        reservedSlots.Clear();
    }

    public bool TryCustomerGrabProduct(ProductWorldSlot slot)
    {
        double priceThreshold = slot.ProductInSlot.ProductData.baseCost * Random.Range(1.05f, 1.5f);

        float randomChance = Random.value;

        double priceDifference = slot.currentlySetPrice - slot.ProductInSlot.ProductData.baseCost;

        float noGrabProbability = Mathf.Lerp(0.09f, 0.01f, Mathf.Clamp01((float)(priceDifference / slot.ProductInSlot.ProductData.baseCost)));

        if (randomChance < noGrabProbability)
        {
            // Log customer thoughts or any other action
            // Customer decides not to grab the product
            return false;
        }

        if (slot.currentlySetPrice > priceThreshold)
        {
            // Log customer thoughts or any other action
            // Product is too expensive
            return false;
        }

        // Customer decides to grab the product
        CustomerGrabProduct(slot);
        return true;
    }

    public void CustomerGrabProduct(ProductWorldSlot slot) {
        slot.MarkReserved();
    }

    public void OpenBargainingMenu() {
        Singleton<PlayerController>.Instance.StartBarterWithShopper(this);
    }

    public IEnumerator HandleLeaving()
    {
        if (atRegisterTable != null)
        {
            atRegisterTable.RemoveShopper(this);
        }

        yield return character.SetNewDestination(shopManager.ExitPoint.position, shopManager.ExitPoint.position);

        shopManager.DespawnShopperCustomer(this);

        activeCoroutine = null;
    }

    public IEnumerator BrowseShop() {
        while(State == ShopperState.BrowsingShop) {

            if (reservedSlots.Count > 0)
            {
                // Check if there are reserved slots.
                float chanceToLeave = Mathf.Clamp01((float)reservedSlots.Count / (float)shopManager.DisplaySlots.Count);
                // Increase the chance if the shopper has less currentMoney compared to their original amount.
                float moneyRatio = currentMoney / originalMoney; // Assuming 2000 is the original amount of money.
                chanceToLeave *= moneyRatio;

                // Implement a low random chance to transition to State = ShopperState.ReadyToLeave.
                if (Random.value < chanceToLeave)
                {
                    State = ShopperState.ReadyToLeave;
                    continue;
                }
            }

            Debug.LogError("BrowseShop - Find Random Slot");

            ProductWorldSlot random_slot = shopManager.DisplaySlots
                .Where(s => 
                    !s.isSelectedByNPC && 
                    s.ProductInSlot != null && 
                    !s.IsReserved() && 
                    s.ProductInSlot.currentlySetPrice > 0 && 
                    s.ProductInSlot.currentlySetPrice <= currentMoney &&
                    !browsedSlots.Contains(s)
                ) .OrderBy(s => Guid.NewGuid()) .FirstOrDefault();

            if(random_slot == null) {
                State = ShopperState.ReadyToLeave;
                continue;
            }

            browsedSlots.Add(random_slot);

            random_slot.isSelectedByNPC = true;

            Debug.LogError("BrowseShop - Moving To Slot");

            yield return character.SetNewDestination(random_slot.transform.position, random_slot.customerStandPosition.position);
            yield return character.PlayEmote("Wonder");

            if(TryCustomerGrabProduct(random_slot)) {
                reservedSlots.Add(random_slot);
                yield return character.PlayEmote("Grab");
                currentMoney -= random_slot.ProductInSlot.currentlySetPrice;
                yield return new WaitForSecondsRealtime(.5f);
            } else {
                yield return character.PlayEmote("Dismiss");
                yield return new WaitForSecondsRealtime(.5f);
            }

            random_slot.isSelectedByNPC = false;
        }

        activeCoroutine = null;
    }

    public bool CanInteract()
    {
        return State == ShopperState.WaitingAtRegister;
    }
}