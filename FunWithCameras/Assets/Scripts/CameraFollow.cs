using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float angle;

    [SerializeField]
    private float distance;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private float obstructionCheckAccuracy = 0.3f;

    private bool obstructed;
    private float alteredDistance;
    private Vector3[,] obstructedLines;
    private int currentGizmoLineIndex;

    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
        }
    }

    public float Distance
    {
        get
        {
            if (obstructed)
            {
                return alteredDistance;
            }
            else
            {
                return distance;
            }
        }
        set
        {
            if (obstructed)
            {
                alteredDistance = value;
            }

            distance = value;
        }
    }

    private void Start()
    {
        obstructedLines = new Vector3[ 5, 2 ];
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            GoToPosition();
        }
    }

    private void GoToPosition()
    {
        Vector3 newPosition;
        Quaternion newRotation = transform.rotation;
        float angleRad = Mathf.Deg2Rad * angle;

        obstructed = false;

        float forward = Mathf.Sin(angleRad) * distance;
        float up = Mathf.Cos(angleRad) * distance;

        newPosition = target.position - (target.forward * forward);
        newPosition += Vector3.up * up;

        newPosition = NonObstructedPoint(newPosition);

        if (obstructed)
        {
            currentGizmoLineIndex++;
        }

        float angleX = -1 * angle + 90;

        newRotation = Quaternion.Euler(angleX, target.rotation.eulerAngles.y, 0);

        transform.position = newPosition;
        transform.rotation = newRotation;
    }

    private Vector3 NonObstructedPoint(Vector3 camPosition)
    {
        Ray ray = new Ray(target.position, camPosition - target.position);
        RaycastHit hitInfo;
        bool result = Physics.SphereCast
            (ray, 0.01f, out hitInfo, distance, mask);

        if (result)
        {
            obstructed = true;

            UpdateGizmoLines();

            Vector3 newPosition =
                hitInfo.point + (target.position - camPosition).normalized
                * obstructionCheckAccuracy;

            alteredDistance = Vector3.Distance(target.position, newPosition);

            return newPosition;
        }

        return camPosition;
    }

    private void UpdateGizmoLines()
    {
        if (currentGizmoLineIndex >= obstructedLines.GetLength(0))
        {
            currentGizmoLineIndex = 0;
        }

        obstructedLines[currentGizmoLineIndex, 0] = target.position;
        obstructedLines[currentGizmoLineIndex, 1] = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(target.position, transform.position);

        if (obstructedLines != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < obstructedLines.GetLength(0); i++)
            {
                if (obstructedLines[i, 0] != null)
                {
                    Gizmos.DrawLine(obstructedLines[i, 0], obstructedLines[i, 1]);
                }
            }
        }
    }
}
