using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSign : MonoBehaviour
{
    [SerializeField] private Transform RedCube;
    [SerializeField] private Transform GreenCube;

    [SerializeField] private bool _isOpen = false;

    public bool IsOpen {
        get {
            return _isOpen;
        }
        set {
            _isOpen = value;
            RedCube.gameObject.SetActive(!_isOpen);
            GreenCube.gameObject.SetActive(_isOpen);
        }
    }

    public void ToggleOpen() {
        IsOpen = !IsOpen;
    }

    void OnMouseDown() {
        ToggleOpen();
    }
}
