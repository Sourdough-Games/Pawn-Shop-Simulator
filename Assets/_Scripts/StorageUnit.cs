using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StorageUnit : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Transform SpawnArea;

    [SerializeField] private AudioSource openSound;

    GarageDoor garageDoor;

    private List<Product> spawnedProducts = new List<Product>();

    public bool CanSpawn = false;

    public bool IsOwned = false;

    public float currentPrice = 0;

    public bool IsOpen {
        get {
            return doorAnimator.GetBool("IsOpen");
        }
    }

    void Start() {
        garageDoor = GetComponentInChildren<GarageDoor>();
    }

    public void OpenUnit() {
        if (!doorAnimator.GetBool("IsOpen") && !garageDoor.isOpening) {
            SpawnProducts();
            doorAnimator.Play("OpenDoor");
            openSound.Play();

            garageDoor.isOpening = true;
        }
    }
    
    public void CloseUnit(bool doAnimation = true) {
        if (doorAnimator.GetBool("IsOpen") && garageDoor.isOpening) {
            doorAnimator.Play("CloseDoor");
            openSound.Play();

            garageDoor.isOpening = false;
        }
    }

    private void SpawnProducts(int limit = 15)
    {
        if (!CanSpawn)
        {
            Debug.LogError("Cant Spawn Products");
            return;
        }

        Debug.LogError("Spawn Products");

        ProductSO[] products = Singleton<ProductManager>.Instance.Products;
        PlayerController c = Singleton<PlayerController>.Instance;

        Bounds bounds = SpawnArea.GetComponent<Collider>().bounds;

        for (int i = 0; i < Random.Range(c.StorageUnitProductSpawnMin, c.StorageUnitProductSpawnMax); i++)
        {
            Vector3 randomPosition = Helper.GetRandomPositionWithinBounds(bounds);
            ProductSO randomProduct = GetRandomProduct();

            Quaternion randomRotation;

            if (randomProduct.productSize == ProductSize.Large)
            {
                // For large products, only rotate on x and y axes
                float randomAngleY = Random.Range(0f, 360f);
                randomRotation = Quaternion.Euler(0f, randomAngleY, 0f);
            }
            else
            {
                // For other products, rotate randomly in all axes
                randomRotation = Random.rotation;
            }

            GameObject obj = Instantiate(randomProduct.prefab, randomPosition, randomRotation);
            Product product = obj.GetComponent<Product>();

            spawnedProducts.Add(product);
        }

        CanSpawn = false;
    }

    private ProductSO GetRandomProduct()
    {
        ProductSO[] products = Singleton<ProductManager>.Instance.Products;

        var sortedProducts = products.OrderBy(p => p.baseCost).ToArray();

        // Calculate total weight
        float totalWeight = sortedProducts.Sum(p => 1 / p.baseCost); // Inverse of baseCost

        // Generate a random number within the total weight
        float randomWeight = Random.Range(0f, totalWeight);

        // Iterate through sorted products and select one based on weighted probability
        ProductSO randomProduct = sortedProducts.FirstOrDefault(p =>
        {
            randomWeight -= 1 / p.baseCost; // Inverse of baseCost
            return randomWeight <= 0;
        });

        // Ensure a product is chosen even if totalWeight is very low
        if (randomProduct == null)
        {
            randomProduct = sortedProducts[Random.Range(0, sortedProducts.Length)];
        }

        return randomProduct;
    }

}
