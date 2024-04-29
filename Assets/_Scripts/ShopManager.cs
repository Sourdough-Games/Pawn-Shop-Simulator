using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ShopManager : MonoBehaviour
{   
    [SerializeField] private GameObject[] shopperPrefabs;

    [SerializeField] private Transform customerSpawnPoint;
    [SerializeField] private Transform customerExitPoint;

    private OpenSign openSign;

    private ProductWorldSlot[] shopSlots;

    public int minCustomerMoney = 500;

    public int maxCustomerMoney = 5000;

    public int maxCustomersAtOnce = 1;

    private List<ShopperNPC> activeShoppers = new();

    public List<ProductWorldSlot> DisplaySlots {
        get {
            return shopSlots.ToList();
        }
    }

    public Transform ExitPoint {
        get {
            return customerExitPoint;
        }
    }

    private RegisterTable[] registerTables;

    public List<RegisterTable> RegisterTables
    {
        get
        {
            return registerTables.ToList();
        }
    }

    void Start() {
        openSign = GetComponentInChildren<OpenSign>();

        shopSlots = GetShopDisplays();
        registerTables = GetRegisterTables();

        StartCoroutine(SpawnJob());
    }

    private IEnumerator SpawnJob() {
        while(true) {
            yield return new WaitForSecondsRealtime(10f);

            if (!openSign.IsOpen) {
                Debug.LogError("Shop is not open, will not spawn npc");
                continue;
            }

            if (activeShoppers.Count < maxCustomersAtOnce && DisplaySlots.Where(s => s.currentlySetPrice > 0 && !s.IsReserved()).Any()) {
                GameObject obj = Instantiate(shopperPrefabs[Random.Range(0, shopperPrefabs.Length)], customerSpawnPoint);
                obj.transform.localPosition = new Vector3(0, 0, 0);

                var npc = obj.GetComponent<ShopperNPC>();
                activeShoppers.Add(npc);

                npc.Setup(this, Random.Range(minCustomerMoney, maxCustomerMoney));
            }
        }
    }

    public void DespawnShopperCustomer(ShopperNPC shopper) {
        activeShoppers.Remove(shopper);

        shopper.ReservedSlots.ForEach(s => s.Unreserve());

        Destroy(shopper.gameObject);
    }

    private ProductWorldSlot[] GetShopDisplays() {
        return GetComponentsInChildren<ProductWorldSlot>(true);
    }

    private RegisterTable[] GetRegisterTables()
    {
        return GetComponentsInChildren<RegisterTable>(true);
    }
}
