using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUnit : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Transform SpawnArea;

    [SerializeField] private AudioSource openSound;

    private List<Product> spawnedProducts = new List<Product>();

    public bool CanSpawn = false;

    public bool IsOwned = false;

    public bool IsOpen {
        get {
            return doorAnimator.GetBool("IsOpen");
        }
    }

    public void OpenUnit() {
        if (!doorAnimator.GetBool("IsOpen")) {
            SpawnProducts();
            doorAnimator.Play("OpenDoor");
            openSound.Play();
        }
    }
    
    public void CloseUnit(bool doAnimation = true) {
        if (doorAnimator.GetBool("IsOpen")) {
            doorAnimator.Play("CloseDoor");
            openSound.Play();
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
            ProductSO randomProduct = products[Random.Range(0, products.Length)];

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

}
