using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    private Animator animator;    
    void Awake() {
        animator = GetComponent<Animator>();
    }

    public void IsOpen() {
        animator.SetBool("IsOpen", true);
    }

    public void IsClosed()
    {
        animator.SetBool("IsOpen", false);
    }
}
