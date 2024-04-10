using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public UnityEvent CharacterClicked;

    [SerializeField] private Outline ot;

    bool PlayerIsWithinDistance
    {
        get
        {
            float distance = Vector3.Distance(transform.position, Singleton<PlayerObjectHolder>.Instance.transform.position);

            // Check if the distance is within the allowed range
            return distance <= Singleton<PlayerObjectHolder>.Instance.reachDistance;
        }
    }

    private void OnMouseDown() {
        if(PlayerIsWithinDistance) {
            CharacterClicked?.Invoke();
        }
    }

    private void OnMouseOver() {
        if(ot != null && PlayerIsWithinDistance) {
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
