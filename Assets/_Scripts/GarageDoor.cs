using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    private Animator animator;    
    private StorageUnit unit;
    private Outline ot;

    public bool isOpening = false;

    public bool isStatic = false;

    private PlayerController controller;

    public StorageUnit Unit {
        get {
            return unit;
        }
    }

    void Awake() {
        animator = GetComponent<Animator>();
        unit = GetComponentInParent<StorageUnit>();
        ot = GetComponent<Outline>();
    }

    void Start() {
        controller = Singleton<PlayerController>.Instance;
    }

    public void Update() {
        if(isStatic) return;

        if(unit.IsOwned && !unit.IsOpen) {
            ot.OutlineColor = Color.green;
            ot.enabled = true;
            return;
        }

        ot.OutlineColor = Color.white;
    }

    public void IsOpen() {
        ot.enabled = false;
        animator.SetBool("IsOpen", true);
    }

    public void IsClosed()
    {
        animator.SetBool("IsOpen", false);
    }

    public void OnMouseDown() {
        if(isStatic) return;

        if(controller.CanInteractWithTransform(transform, additional_conditions: unit.IsOwned)) {
            if(unit.IsOpen) {
                unit.CloseUnit();
            } else {
                unit.OpenUnit();
            }
        }
    }
}
