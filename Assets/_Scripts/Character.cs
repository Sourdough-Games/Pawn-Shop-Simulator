using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{
    public UnityEvent CharacterClicked;

    [SerializeField] private Outline ot;
    bool PlayerIsWithinDistance
    {
        get
        {
            return Helper.IsWithinPlayerReach(transform);
        }
    }

    void Update() {
        if(ot.enabled && Singleton<PlayerController>.Instance.openModal != null || !PlayerIsWithinDistance) {
            ot.enabled = false;
        }
    }

    private void OnMouseDown() {
        if(PlayerIsWithinDistance && Singleton<PlayerController>.Instance.openModal == null) {
            CharacterClicked?.Invoke();
        }
    }

    private void OnMouseOver() {
        if(ot != null) {
            ot.enabled = true;
        }
    }

    private void OnMouseExit()
    {
        if (ot != null)
        {
            ot.enabled = false;
        }
    }
}
