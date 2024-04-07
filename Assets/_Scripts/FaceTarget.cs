using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
