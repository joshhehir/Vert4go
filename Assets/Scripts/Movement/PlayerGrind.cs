using FPSController;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class PlayerGrind : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] bool jump;         // Inputs aren't used in the tutorial
    [SerializeField] Vector3 input;     // But they're here for rail switching
    Vector3 railExitVelocity;
    Vector3 exitDirection;  // Direction at the point of leaving the rail
    [SerializeField] float gravity = 9.8f;  // Gravity for free fall
    Vector3 velocity;  // Current velocity (momentum and gravity)
    bool isFlinging = false;  // To track if the player is in mid-air
    private Vector3 lastValidDirection;
    private InputHandler inputHandler; // Reference to your InputHandler
    private bool onRail; // This should be set when the player is on the rail

    [Header("Variables")]
    [SerializeField] float grindSpeed;
    [SerializeField] float heightOffset;
    float timeForFullSpline;
    float elapsedTime;
    [SerializeField] float lerpSpeed = 10f;

    [Header("Scripts")]
    [SerializeField] RailScript currentRailScript;
    Rigidbody playerRigidbody;
    CharacterController charController;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
        inputHandler = FindObjectOfType<InputHandler>(); // Get reference to InputHandler
    }

    public void HandleMovement(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        input.x = rawInput.x;
    }

    private void FixedUpdate()
    {
        if (onRail) // If on the rail, move the player along the rail
        {
            MovePlayerAlongRail();
        }
    }

    private void Update()
    {
        if (isFlinging)
        {
            // Apply gravity to the velocity
            velocity.y -= gravity * Time.deltaTime;

            // Move the player based on the velocity
            charController.Move(velocity * Time.deltaTime);

            // Stop fling behavior if the player lands
            if (charController.isGrounded)
            {
                isFlinging = false;
                velocity = Vector3.zero;
            }
        }
    }

    private void JumpOffRail()
    {
        // Set onRail to false
        onRail = false;

        // Clear current rail script
        currentRailScript = null;

        // Apply a jump force while maintaining momentum
        velocity += Vector3.up * jumpForce; // Define jumpForce to control jump height
        isFlinging = true; // Enable fling behavior
    }

    void MovePlayerAlongRail()
    {
        float3 pos, tangent, up;
        float3 nextPosfloat, nextTan, nextUp;

        if (currentRailScript != null && onRail)
        {
            float progress = elapsedTime / timeForFullSpline;

            // Detect if player is at start or end of the rail
            if (progress <= 0.01f || progress >= 0.99f)
            {
                // Evaluate position and tangent at the endpoint
                SplineUtility.Evaluate(currentRailScript.railSpline.Spline, Mathf.Clamp01(progress), out pos, out tangent, out up);

                // Determine exit direction based on rail type
                if (currentRailScript.IsVerticalRail)
                {
                    exitDirection = Vector3.up;  // Launch straight up for vertical rails
                }
                else
                {
                    // Use the tangent direction for horizontal rails
                    // Get the tangent at the player's current position
                    Vector3 railTangent = currentRailScript.LocalToWorldConversion(tangent).normalized;

                    // Determine the player's current velocity relative to the rail
                    Vector3 playerVelocity = playerRigidbody.velocity;

                    // Calculate the dot product to check if the player is moving along or against the rail's tangent
                    float directionDot = Vector3.Dot(playerVelocity, railTangent);

                    // If the player is moving against the tangent, invert the rail direction
                    if (directionDot < 0)
                    {
                        railTangent = -railTangent;
                    }

                    // Set the exit direction to the rail tangent
                    exitDirection = railTangent;
                }

                // Apply the exit velocity
                velocity = exitDirection * grindSpeed;
                isFlinging = true;  // Enable fling behavior
                ThrowOffRail();  // Exit the rail
                return;
            }

            // Calculate next progress point
            float nextTimeNormalised = currentRailScript.normalDir
                ? (elapsedTime + Time.deltaTime) / timeForFullSpline
                : (elapsedTime - Time.deltaTime) / timeForFullSpline;

            // Evaluate positions and tangents
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, progress, out pos, out tangent, out up);
            SplineUtility.Evaluate(currentRailScript.railSpline.Spline, nextTimeNormalised, out nextPosfloat, out nextTan, out nextUp);

            Vector3 worldPos = currentRailScript.LocalToWorldConversion(pos);
            Vector3 nextPos = currentRailScript.LocalToWorldConversion(nextPosfloat);

            // Update player position with height offset
            transform.position = worldPos + (transform.up * heightOffset);

            // Smoothly rotate towards the next position
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(nextPos - worldPos),
                lerpSpeed * Time.deltaTime
            );

            // Align player's up direction with the rail’s up vector
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.FromToRotation(transform.up, up) * transform.rotation,
                lerpSpeed * Time.deltaTime
            );

            // Store the last valid movement direction
            lastValidDirection = (nextPos - worldPos).normalized;

            // Update elapsed time
            elapsedTime = currentRailScript.normalDir ? elapsedTime + Time.deltaTime : elapsedTime - Time.deltaTime;
        }
    }


    [SerializeField] float jumpForce = 5f; // Adjust this value for desired jump height

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if the collided object has either rail tag
        if (hit.gameObject.CompareTag("VerticalRail") || hit.gameObject.CompareTag("HorizontalRail"))
        {
            onRail = true;
            currentRailScript = hit.gameObject.GetComponent<RailScript>();

            if (currentRailScript != null)
            {
                CalculateAndSetRailPosition();
            }
        }
    }

    void CalculateAndSetRailPosition()
    {
        // Figure out the amount of time it would take for the player to cover the rail.
        timeForFullSpline = currentRailScript.totalSplineLength / grindSpeed;

        // This is going to be the world position of where the player is going to start on the rail.
        Vector3 splinePoint;

        // The 0 to 1 value of the player's position on the spline. We also get the world position of where that point is.
        float normalisedTime = currentRailScript.CalculateTargetRailPoint(transform.position, out splinePoint);
        elapsedTime = timeForFullSpline * normalisedTime;
        // Multiply the full time for the spline by the normalised time to get elapsed time. This will be used in the movement code.

        // Spline evaluate takes the 0 to 1 normalised time above, and uses it to give you a local position, a tangent (forward), and up
        float3 pos, forward, up;
        SplineUtility.Evaluate(currentRailScript.railSpline.Spline, normalisedTime, out pos, out forward, out up);
        // Calculate the direction the player is going down the rail
        currentRailScript.CalculateDirection(forward, transform.forward);
        // Set player's initial position on the rail before starting the movement code.
        transform.position = splinePoint + (transform.up * heightOffset);
    }

    void ThrowOffRail()
    {
        // Set onRail to false, clear the rail script, and push the player off the rail.
        // It's a little sudden, there might be a better way of doing this using coroutines and looping, but this will work.
        onRail = false;  // Disable rail logic
        currentRailScript = null;  // Clear the current rail
    }
}
