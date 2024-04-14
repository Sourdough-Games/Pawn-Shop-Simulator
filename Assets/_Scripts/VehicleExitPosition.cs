using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleExitPosition : MonoBehaviour
{

    public bool isColliding;

    private void OnTriggerStay(Collider other)
    {
        Product product;
        if(!other.TryGetComponent<Product>(out product)) {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
    }
}
