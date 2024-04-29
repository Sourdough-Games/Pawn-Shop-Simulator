using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;
using UnityEngine.Events;

public class InteractableOutline : Outline
{
    [SerializeField] private List<Canvas> HoverCanvas;
    [SerializeField] private UnityEvent clickEvent;
    [SerializeField] private InterfaceReference<IInteractable> Interactable = new InterfaceReference<IInteractable>();
    private PlayerController controller;

    void Start() {
        if(Interactable == null) {
            Interactable.Value = GetComponent<IInteractable>();
        }
        OnMouseExit();
    }

    private void OnMouseOver() {
        if(controller == null) {
            //Debug.LogWarning("Attempting to find PlayerController singleton...");
            controller = Singleton<PlayerController>.Instance;
            return;
        }
        
        if(!controller.CanInteractWithTransform(transform) || (Interactable.Value != null && !Interactable.Value.CanInteract())) {
            OnMouseExit();
            return;
        }

        if (gameObject.activeSelf) {
            enabled = true;

            HoverCanvas.ForEach(c => c.gameObject.SetActive(true));
        }
    }

    private void OnMouseDown() {
        if(controller == null) {
            controller = Singleton<PlayerController>.Instance;
            return;
        }

        if(!controller.CanInteractWithTransform(transform) || (Interactable.Value != null && !Interactable.Value.CanInteract())) {
            return;
        }

        if(gameObject.activeSelf) {
            clickEvent?.Invoke();
        }
    }

    private void OnMouseExit() {
        enabled = false;

        HoverCanvas.ForEach(c => c.gameObject.SetActive(false));
    }
}
