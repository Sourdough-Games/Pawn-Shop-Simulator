using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckBed : MonoBehaviour
{
    [SerializeField] public Collider bedCollider;

    private void Start() {
        bedCollider.enabled = false;
    }

    void Update() {
        if(IsTruckRunning) {
            bedCollider.enabled = true;
        }
    }

    public bool IsTruckRunning {
        get {
            return GetComponentInParent<CarController>().BeingOperated;
        }
    }
}
