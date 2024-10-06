using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LeverScript : MonoBehaviour
{
    private Quaternion startingRotation; // Store the starting rotation
    private bool isLeverDown = false;
    public float targetAngle = 45f; // Target angle for rotation
    public float rotationSpeed = 5f; // Speed of rotation

    public UnityEvent onLeverPulledDown = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = transform.rotation; // Initialize starting rotation
    }

    void OnMouseDown()
    {
        if(isLeverDown == false)
        {
            isLeverDown = true;
            StartCoroutine(RotateLever());
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && isLeverDown == true)
        {
            isLeverDown = false;
            StartCoroutine(SnapLeverBack());
        }
    }

    private IEnumerator RotateLever()
    {
        // Rotate to target angle
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0, 0) * startingRotation;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            if(!isLeverDown)
            {
                break;
            }
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, elapsedTime);
            elapsedTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        // Check if the lever is close to the target rotation before invoking the event
        if (Quaternion.Angle(transform.rotation, targetRotation) < 10f) // Threshold of 1 degree
        {
            onLeverPulledDown.Invoke();
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