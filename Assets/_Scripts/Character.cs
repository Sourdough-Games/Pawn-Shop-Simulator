using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour
{
    public UnityEvent CharacterClicked;

    [SerializeField] private Outline ot;

    private void OnMouseDown() {
        if(Singleton<PlayerController>.Instance.CanInteractWithTransform(transform)) {
            CharacterClicked?.Invoke();
        }
    }
}
