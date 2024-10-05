using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    public float rotationSpeed = 5f; // How fast the object rotates based on the cursor position
    public float maxRotation = 15f;  // Maximum angle the object can rotate

    private Vector3 initialRotation;

    void Start()
    {
        // Store the initial rotation of the object
        initialRotation = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        // Get the mouse position relative to the screen
        Vector3 mousePosition = Input.mousePosition;

        // Normalize the mouse position (0 to 1) based on screen width and height
        float mouseX = (mousePosition.x / Screen.width) * 2 - 1;
        float mouseY = (mousePosition.y / Screen.height) * 2 - 1;

        // Calculate the target rotation based on the mouse position
        float targetRotationX = Mathf.Clamp(-mouseY * maxRotation, -maxRotation, maxRotation);
        float targetRotationY = Mathf.Clamp(mouseX * maxRotation, -maxRotation, maxRotation);

        // Create the target rotation using Euler angles
        Quaternion targetRotation = Quaternion.Euler(targetRotationX, targetRotationY, 0);

        // Smoothly rotate the object towards the target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
