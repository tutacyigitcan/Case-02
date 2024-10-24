using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offset;

    private void Start()
    {
        offset = transform.position - cameraTransform.position;
    }

    private void Update()
    {
        transform.position = cameraTransform.position + offset;
    }
}
