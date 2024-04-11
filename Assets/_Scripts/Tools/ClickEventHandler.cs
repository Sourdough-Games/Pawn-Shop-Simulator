using UnityEngine;
using UnityEngine.Events;

public class ClickEventHandler : MonoBehaviour
{
    public UnityEvent runEvent;

    [SerializeField] private Outline ol;

    void Start() {
        if(ol) {
            ol.enabled = false;
        }
    }

    public void OnMouseDown()
    {
        runEvent?.Invoke();
    }

    public void OnMouseEnter() {
        if(ol != null) {
            ol.enabled = true;
        }
    }

    public void OnMouseExit()
    {
        if (ol != null)
        {
            ol.enabled = false;
        }
    }
}