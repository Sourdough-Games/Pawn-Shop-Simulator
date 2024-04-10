using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private EscapeMenuUI escapeMenu;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && !EventSystem.current.IsPointerOverGameObject()) {
            escapeMenu.Open();
        }
    }   
}
