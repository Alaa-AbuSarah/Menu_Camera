using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Camera : MonoBehaviour
{
    #region Inputs
    private enum CameraType { Mouse, Touch }
    [SerializeField] private CameraType type = CameraType.Mouse;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 smoothVelocity = Vector3.zero;
    private Vector3 mouseLastPosition = Vector3.zero;
    private Vector2 touchLastPosition = Vector2.zero;
    private Vector3 direction = Vector3.zero;
    const float threshold = 9;
    #endregion

    [Space]

    #region Target
    [SerializeField] private Transform target;
    [SerializeField] private float distanceFromTarget = 10f;

    private Vector3 currentRotation;
    #endregion

    [Space]

    #region Yaw
    [Tooltip("The amount of vertical rotation")]
    [SerializeField] private bool yaw = true;
    [Range(-180, 180)] [SerializeField] private float yawMin = 0f;
    [Range(-180, 180)] [SerializeField] private float yawMax = 40f;

    private float rotationX;
    #endregion

    [Space]

    #region Pitch
    [Tooltip("The amount of horizontal rotation")]
    [SerializeField] private bool pitch = false;
    [Range(-180, 180)] [SerializeField] private float pitchMin = 0f;
    [Range(-180, 180)] [SerializeField] private float pitchMax = 40f;

    private float rotationY;
    #endregion

    private void OnValidate()
    {
        if (yawMin > yawMax) yawMin = yawMax;//Ensure that the yawMin is less than the yawMax.
        if (pitchMin > pitchMax) pitchMin = pitchMax;//Ensure that the pitchMin is less than the pitchMax.
    }

    private void Update()
    {
        switch (type)
        {
            case CameraType.Mouse:
                direction = MouseDirection();
                break;
            case CameraType.Touch:
                direction = TouchDirection();
                break;
        }

        rotationY += direction.x * sensitivity;
        rotationX += direction.y * sensitivity;

        // Apply yaw for vertical rotation 
        if (yaw) rotationX = Mathf.Clamp(rotationX, yawMin, yawMax);

        // Apply pitch for horizontal rotation 
        if (pitch) rotationY = Mathf.Clamp(rotationY, pitchMin, pitchMax);

        Vector3 nextRotation = new Vector3(rotationX, rotationY);

        // Apply damping between rotation changes
        currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);
        transform.localEulerAngles = currentRotation;

        // Substract forward vector of the GameObject to point its forward vector to the target
        transform.position = target.position - transform.forward * distanceFromTarget;
    }

    /// <summary>
    /// Calculate the direction of movement of the mouse.
    /// </summary>
    /// <returns>Mouse movement direction</returns>
    private Vector2 MouseDirection()
    {
        Vector2 dir = Vector2.zero;

        Vector3 movementDirection = Input.mousePosition - mouseLastPosition;
        movementDirection.Normalize();

        float movementValue = 0f;

        //Ensure that the distance is sufficient to start moving.
        if (Vector3.Distance(mouseLastPosition, Input.mousePosition) < threshold) return dir;

        //Movement value vertical.
        movementValue = Vector3.Dot(movementDirection, Vector3.up);
        if (movementValue >= 0.5)
        {
            dir.y = -1;
        }
        else if (movementValue <= -0.5)
        {
            dir.y = 1;
        }

        //Movement value horizontal.
        movementValue = Vector3.Dot(movementDirection, Vector3.right);
        if (movementValue >= 0.5)
        {
            dir.x = 1;
        }
        else
        {
            dir.x = -1;
        }

        mouseLastPosition = Input.mousePosition;

        return dir;
    }

    /// <summary>
    /// Calculate the direction of movement of the touch.
    /// </summary>
    /// <returns>Touch movement direction</returns>
    private Vector2 TouchDirection()
    {
        Vector2 dir = Vector2.zero;

        //Make sure there is touch on the screen.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);//Recording first touch.

            Vector2 movementDirection = touch.position - touchLastPosition;
            movementDirection.Normalize();

            float movementValue = 0f;

            //Ensure that the distance is sufficient to start moving.
            if (Vector3.Distance(touchLastPosition, touch.position) < threshold) return dir;

            //Movement value vertical.
            movementValue = Vector3.Dot(movementDirection, Vector3.up);
            if (movementValue >= 0.5)
            {
                dir.y = -1;
            }
            else if (movementValue <= -0.5)
            {
                dir.y = 1;
            }

            //Movement value horizontal.
            movementValue = Vector3.Dot(movementDirection, Vector3.right);
            if (movementValue >= 0.5)
            {
                dir.x = 1;
            }
            else
            {
                dir.x = -1;
            }

            touchLastPosition = touch.position;
        }

        return dir;
    }
}
