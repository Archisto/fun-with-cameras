using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.y = 0;
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
