using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public float moveSpeed = 1;
    public float runSpeed = 3;
    public float rotationSpeed = 1;
    public float jumpForce = 1;
    public float gravity = -9.81f;

    public bool invertHorizontal;
    public bool invertVertical;

    public float cameraMinAngle = 0f;
    public float cameraMaxAngle = 90f;
    public float cameraMaxAngleFPMode = 180f;
    public float cameraMinDist = 2f;
    public float cameraMaxDist = 30f;
    public float firstPersonDist = 0.5f;
    public float cameraAngleSpeed = 1;
    public float cameraDistSpeed = 1;

    private Vector3 movement;
    private float movementY;

    private bool jumped;

    private CharacterController charController;
    private Renderer renderer;
    private CameraFollow playerCamera;

    // Use this for initialization
    private void Start ()
    {
        Vector3 movement = Vector3.zero;
        charController = GetComponent<CharacterController>();
        renderer = GetComponent<Renderer>();
        playerCamera = FindObjectOfType<CameraFollow>();
    }

    // Update is called once per frame
    private void Update ()
    {
        movement = Vector3.zero;

        Move();
        Jump();
        Turn();
        ControlCamera();

        charController.Move(movement * Time.deltaTime);

        /*
        float turnSmoothing = 1;

        Vector3 trajectory = Vector3.zero;
        trajectory.y = jumpForce;
        trajectory.y -= gravity;
        charController.Move(trajectory * Time.deltaTime);

        trajectory.y = 0.0f;
        if (trajectory != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(trajectory, Vector3.up);
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRot, turnSmoothing * Time.deltaTime);
            transform.rotation = newRotation;
        }
        */
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            float altMoveSpeed = moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                altMoveSpeed = runSpeed;
            }

            movement += altMoveSpeed * transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += moveSpeed * transform.forward * -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += moveSpeed * transform.right * -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += moveSpeed * transform.right;
        }
    }

    private void Jump()
    {
        bool jumpButtonDown = Input.GetKey(KeyCode.Space);

        if (charController.isGrounded)
        {
            movementY = 0;

            if (!jumped && jumpButtonDown)
            {
                movementY = jumpForce;
                jumped = true;
            }
        }
        else
        {
            movementY += gravity;
        }

        if (jumped && !jumpButtonDown)
        {
            jumped = false;
        }

        movement.y = movementY;
    }

    private void Turn()
    {
        bool turnLeft = Input.GetKey(KeyCode.LeftArrow);
        bool turnRight = Input.GetKey(KeyCode.RightArrow);

        if (turnLeft || turnRight)
        {
            if (invertHorizontal)
            {
                bool temp = turnLeft;
                turnLeft = turnRight;
                turnRight = temp;
            }

            Vector3 newRotation = transform.rotation.eulerAngles;

            if (turnLeft)
            {
                newRotation.y += rotationSpeed * -1f * Time.deltaTime;
            }
            if (turnRight)
            {
                newRotation.y += rotationSpeed * Time.deltaTime;
            }

            transform.rotation = Quaternion.Euler(newRotation);
        }
    }

    private void ControlCamera()
    {
        float cameraAngleDelta = 0;

        bool lookUp = Input.GetKey(KeyCode.UpArrow);
        bool lookDown = Input.GetKey(KeyCode.DownArrow);

        if (invertVertical)
        {
            bool temp = lookUp;
            lookUp = lookDown;
            lookDown = temp;
        }

        if (lookUp && playerCamera.Angle <
                (FirstPersonMode ? cameraMaxAngleFPMode : cameraMaxAngle))
        {
            cameraAngleDelta += cameraAngleSpeed;
        }
        if (lookDown && playerCamera.Angle > cameraMinAngle)
        {
            cameraAngleDelta += cameraAngleSpeed * -1f;
        }

        playerCamera.Angle += cameraAngleDelta * Time.deltaTime;

        float distanceChange = Input.mouseScrollDelta.y * -1f * cameraDistSpeed * Time.deltaTime;

        if (distanceChange != 0)
        {
            playerCamera.Distance += distanceChange;
            playerCamera.Distance = Mathf.Clamp(playerCamera.Distance, cameraMinDist, cameraMaxDist);

            if (playerCamera.Distance < firstPersonDist)
            {
                if (!FirstPersonMode)
                {
                    FirstPersonMode = true;
                }
            }
            else if (FirstPersonMode)
            {
                FirstPersonMode = false;
            }
        }
    }

    public bool FirstPersonMode
    {
        get
        {
            return !renderer.enabled;
        }
        set
        {
            renderer.enabled = !value;

            if (value == false)
            {
                if (playerCamera.Angle > cameraMaxAngle)
                {
                    playerCamera.Angle = cameraMaxAngle;
                }
            }
        }
    }
}
