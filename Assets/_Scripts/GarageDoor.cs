using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    private Animator animator;    
    private StorageUnit unit;
    private Outline ot;

    public bool isOpening = false;

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
        if(isOpening) {
            ot.OutlineColor = Color.white;
            return;
        }

        if(!isOpening && unit.IsOwned && !unit.IsOpen) {
            ot.OutlineColor = Color.green;
            ot.enabled = true;
        } else {

            ot.OutlineColor = Color.white;
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
                isOpening = false;
                unit.CloseUnit();
            } else {
                Debug.LogError("Open Unit");
                unit.OpenUnit();
                isOpening = true;
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
