using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public float normalMoveSpeed = 5.0f;
    public float sprintSpeed = 5.0f;
    public float currentMoveSpeed = 5.0f;
    private bool isFocused = true;

    public static bool isDuringCamMovement = false;
    private void OnApplicationFocus(bool focus)
    {
        isFocused = focus;
    }
    void Update()
    {
        isDuringCamMovement = false;
        if (!isFocused) return;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            currentMoveSpeed = normalMoveSpeed;
        }

        // Rotation
        if (Input.GetMouseButton(1)) // 1 is the right mouse button
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            if(horizontalRotation > 0 || verticalRotation > 0)
            {
                isDuringCamMovement = true;
            }

            // this will make it so when we refocus we won't do a chaotic jump for the rotation of cam
            // the number 5 is hardcoded after testing - will replace it!
            // maybe will create a variabl that will hold the variable when I unfocus and then we can just reset to that.
            if (Mathf.Abs(horizontalRotation) > 5 || Mathf.Abs(verticalRotation) > 5) return;

            transform.Rotate(Vector3.up, horizontalRotation, Space.World); // Yaw
            transform.Rotate(Vector3.right, -verticalRotation, Space.Self); // Pitch
        }

        // Movement
        float horizontalMove = Input.GetAxis("Horizontal") * currentMoveSpeed * Time.deltaTime;
        float verticalMove = Input.GetAxis("Vertical") * currentMoveSpeed * Time.deltaTime;

        if(horizontalMove > 0 || verticalMove > 0)
        {
            isDuringCamMovement = true;
        }

        Vector3 moveDirection = transform.forward * verticalMove + transform.right * horizontalMove;
        transform.position += moveDirection;

        // Optional: Up and Down movement (with 'Q' and 'E')
        if (Input.GetKey(KeyCode.Q))
        {
            isDuringCamMovement = true;
            transform.position += Vector3.down * normalMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            isDuringCamMovement = true;
            transform.position += Vector3.up * normalMoveSpeed * Time.deltaTime;
        }
    }
}
