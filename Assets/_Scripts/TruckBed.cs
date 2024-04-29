using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TruckBed : MonoBehaviour
{
    public Action<bool> TruckBedChanged;

    [SerializeField] public Collider bedCollider;

    private CarController carController;

    private void Start() {
        bedCollider.enabled = false;

        carController = GetComponentInParent<CarController>();

        carController.BeingOperatedChanged += b => {
            bedCollider.enabled = b;

            StartCoroutine(Helper.Wait(.1f, () => {
                Debug.LogError($"Trigger TruckBedChanged");
                TruckBedChanged?.Invoke(b);
            }));
        };
    }

    
}
