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

    void Awake() {
        animator = GetComponent<Animator>();
        unit = GetComponentInParent<StorageUnit>();
        ot = GetComponent<Outline>();
    }

    bool PlayerIsWithinDistance
    {
        get
        {
            return Helper.IsWithinPlayerReach(transform, 1f);
        }
    }

    public void Update() {
        if(isStatic) return;
        if (isOpening) {
            ot.OutlineColor = Color.white;
            return;
        }

        if(!isOpening && unit.IsOwned && !unit.IsOpen) {
            ot.OutlineColor = Color.green;
            ot.enabled = true;
        } else {
            ot.OutlineColor = Color.white;

            if(!PlayerIsWithinDistance || !unit.IsOwned) {
                ot.enabled = false;
            }
        }
    }

    public void IsOpen() {
        animator.SetBool("IsOpen", true);
    }

    public void IsClosed()
    {
        animator.SetBool("IsOpen", false);
    }

    public void OnMouseDown() {
        
        if(PlayerIsWithinDistance && unit.IsOwned) {
            if (unit.IsOpen)
            {
                ot.enabled = false;
                unit.CloseUnit();
            } else {
                Debug.LogError("Open Unit");
                unit.OpenUnit();
                ot.enabled = false;
            }
        }
    }

    public void OnMouseOver() {
        if(PlayerIsWithinDistance) {
            ot.enabled = true;
        }
    }

    public void OnMouseExit()
    {
        ot.enabled = false;
    }
}
