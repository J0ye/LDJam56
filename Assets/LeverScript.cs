using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class LeverScript : MonoBehaviour
{
    private Quaternion startingRotation; // Store the starting rotation
    public float targetAngle = 45f; // Target angle for rotation
    public float rotationSpeed = 5f; // Speed of rotation

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = transform.rotation; // Initialize starting rotation
    }

    void OnMouseDown()
    {
        StartCoroutine(RotateLever());
    }

    void OnMouseUp()
    {
        StartCoroutine(SnapLeverBack());
    }

    private IEnumerator RotateLever()
    {
        // Rotate to target angle
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0) * startingRotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }
    }

    private IEnumerator SnapLeverBack()
    {
        // Rotate to target angle
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0) * startingRotation;
        // Snap back to starting rotation with increased speed
        float snapBackSpeed = rotationSpeed * 5f; // Speed for snap back
        float snapBackElapsedTime = 0f;
        Quaternion snapBackTarget = startingRotation;

        while (snapBackElapsedTime < 1f)
        {
            transform.rotation = Quaternion.Slerp(targetRotation, snapBackTarget, snapBackElapsedTime);
            snapBackElapsedTime += Time.deltaTime * snapBackSpeed;
            yield return null;
        }

        // Ensure final rotation is exactly the starting rotation
        transform.rotation = startingRotation;
    }
}