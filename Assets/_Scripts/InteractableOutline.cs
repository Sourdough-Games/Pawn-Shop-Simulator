using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableOutline : Outline
{
    [SerializeField] private List<Canvas> ShowAdditionalCanvases;
    private PlayerController controller;

    void Start() {
        OnMouseExit();
    }

    private void OnMouseOver() {
        if(controller == null) {
            Debug.LogWarning("Attempting to find PlayerController singleton...");
            controller = Singleton<PlayerController>.Instance;
            return;
        }
        
        if(!controller.CanInteractWithTransform(transform) || !ComponentCheckPass()) {
            OnMouseExit();
            return;
        }

        if (gameObject.activeSelf) {
            enabled = true;

            ShowAdditionalCanvases.ForEach(c => c.gameObject.SetActive(true));
        }
    }

    private bool ComponentCheckPass() {
        GarageDoor gDoor;
        if (TryGetComponent<GarageDoor>(out gDoor) && !gDoor.Unit.IsOwned)
        {
            return false;
        }

        return true;
    }

    private void OnMouseExit() {
        enabled = false;

        ShowAdditionalCanvases.ForEach(c => c.gameObject.SetActive(false));
    }
}
