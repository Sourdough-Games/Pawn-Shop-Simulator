using UnityEngine;
using UnityEngine.Events;

public class ClickEventHandler : MonoBehaviour
{
    public UnityEvent runEvent;

    public void OnMouseDown()
    {
        runEvent?.Invoke();
    }
}