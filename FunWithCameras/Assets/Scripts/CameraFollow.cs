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
    private float movingSpeed = 0.3f;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Transform secondaryTarget;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private float distFromObstruction = 0.3f;

    private Vector3 intendedPosition;
    private Quaternion intendedRotation;
    private float elevation;
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
            SetPosAndRotBasedOnDistAndAngle();
            SetElevation();

            Move();
        }
    }

    private void Move()
    {
        transform.position = LerpToNonObstructedPosition();
        transform.rotation = intendedRotation;
    }

    /// <summary>
    /// Sets the intended position and rotation.
    /// If there aren't any obstacles in the way,
    /// the camera uses these values.
    /// </summary>
    private void SetPosAndRotBasedOnDistAndAngle()
    {
        Vector3 position;
        Quaternion rotation = transform.rotation;

        float angleRad = Mathf.Deg2Rad * angle;

        float forward = Mathf.Sin(angleRad) * distance;
        float up = Mathf.Cos(angleRad) * distance;

        position = target.position - (target.forward * forward);
        position += Vector3.up * up;

        float angleX = -1 * angle + 90;

        rotation = Quaternion.Euler(angleX, target.rotation.eulerAngles.y, 0);

        intendedPosition = position;
        intendedRotation = rotation;
    }

    private void SetElevation()
    {

    }

    /// <summary>
    /// Gets a position where the camera
    /// isn't obstructed by any world geometry.
    /// </summary>
    /// <returns>A position for the camera</returns>
    private Vector3 LerpToNonObstructedPosition()
    {
        // An unobstructed position between the player
        // character's head and the intended position
        Vector3 nonObstructedPos = NonObstructedPos(intendedPosition, target, distance);

        // Checks if there aren't any obstaces between the
        // player char's foot and the non-obstructed position
        if (!obstructed)
        {
            float distanceFromSecTarget =
                Vector3.Distance(secondaryTarget.position, nonObstructedPos);

            Vector3 nonObstructedFromSecTargetPos = NonObstructedPos(
                nonObstructedPos, secondaryTarget, distanceFromSecTarget);

            if (obstructed)
            {
                // Projects the non-obstructed foot-to-non-obstructed position
                // to the line between target and the intended position
                nonObstructedPos =
                    target.position +
                    Vector3.Project(nonObstructedFromSecTargetPos - target.position,
                            intendedPosition - target.position);
            }
        }

        // Sets the distance between the target and the non-obstructed position
        if (obstructed)
        {
            alteredDistance = Vector3.Distance(target.position, nonObstructedPos);
        }

        // Projects the current position to the line
        // between target and the intended position
        Vector3 projectedPosition =
            target.position +
            Vector3.Project(transform.position - target.position,
                            intendedPosition - target.position);

        // Lerps between the projected position and the non-obstructed position
        return Vector3.Lerp(projectedPosition, nonObstructedPos, movingSpeed);
    }

    /// <summary>
    /// Gets a non-obstructed position between the
    /// given position and the target's position.
    /// </summary>
    /// <param name="position">A position</param>
    /// <param name="target">A target</param>
    /// <param name="rayDistance">Maximum distance for the raycast</param>
    /// <returns></returns>
    private Vector3 NonObstructedPos(
        Vector3 position, Transform target, float rayDistance)
    {
        Ray ray = new Ray(target.position, position - target.position);

        RaycastHit hitInfo;
        bool result = Physics.SphereCast
            (ray, 0.01f, out hitInfo, rayDistance, mask);

        obstructed = result;

        if (result)
        {
            SetObstructedGizmoLine(target);

            Vector3 newPosition =
                hitInfo.point + (target.position - position).normalized
                * distFromObstruction;

            return newPosition;
        }

        return position;
    }

    public float GetCurrentDistanceFromTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private void SetObstructedGizmoLine(Transform target)
    {
        currentGizmoLineIndex++;
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
                if (obstructedLines[i, 0] != Vector3.zero)
                {
                    Gizmos.DrawLine(obstructedLines[i, 0], obstructedLines[i, 1]);
                }
            }
        }
    }
}
