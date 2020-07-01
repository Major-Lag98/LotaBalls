/**
 * Container for the main camera. 
 * - Tilts on the forward and right axis through input on the vertical and horizontal axis.
 * - Apply movement to play by passing tilt factors
 * - Follow player in the direction of their velocity.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform mCamera;

    [SerializeField]
    private PlayerController player;

    private float horizontalTilt;
    private float verticalTilt;

    private float initialXRotation;

    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private float maxVerticalAngle = 15;
    [SerializeField]
    private float maxHorizontalAngle = 10;
    [SerializeField]
    private float tiltSpeed = 75;

    [SerializeField]
    private float offset = 4;

    private bool useFloorNormal;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = transform.GetChild(0);                // Get Camera from child

        initialXRotation = transform.eulerAngles.x;     // Store initial x rotation
    }

    void FixedUpdate()
    {
        if (player.currentState == PlayerController.State.NORMAL)
        {
            verticalTilt = Input.GetAxis("Vertical");
            horizontalTilt = Input.GetAxis("Horizontal");

            player.Move(verticalTilt, horizontalTilt, transform.right);
        }
    }

    void Update()
    {
        if (player.currentState == PlayerController.State.NORMAL)
        {
            useFloorNormal = player.overRamp;
            CameraTilt();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (player.currentState)
        {
            case PlayerController.State.NORMAL:
                // Follow the player
                FollowTarget();
                break;
        }

    }

    void CameraTilt()
    {
        // Rotate camera container along the x axis when tilting the joystick up or down to give a forward and back tilt effect.
        // The further up the joystick is the higher the angle for target rotation will be and vice versa.
        float scaledVerticalTilt = initialXRotation - (verticalTilt * maxVerticalAngle);

        // Using floor normal adjust the rotation of the camera's x axis at rest.
        float angleBetweenFloorNormal = useFloorNormal ? Vector3.SignedAngle(Vector3.up, player.floorNormal, transform.right) : 0.0f;

        Quaternion targetXRotation = Quaternion.Euler(scaledVerticalTilt + angleBetweenFloorNormal, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetXRotation, tiltSpeed * Time.deltaTime);

        // Rotate camera along the z axis when tilting the joystick left or right to give a left and right tilt effect.
        // The further right the joystick is the higher the angle for target rotation will be and vice versa.
        float scaledHorizontalTilt = Input.GetAxis("Horizontal") * maxHorizontalAngle;

        Quaternion targetZRotation = Quaternion.Euler(mCamera.rotation.eulerAngles.x, mCamera.rotation.eulerAngles.y, scaledHorizontalTilt);

        mCamera.rotation = Quaternion.RotateTowards(mCamera.rotation, targetZRotation, tiltSpeed * Time.deltaTime);
    }

    void FollowTarget()
    {
        

        // Get forward vector minus the y component
        Vector3 vectorA = new Vector3(transform.forward.x, 0.0f, transform.forward.z);

        // Get target's velocity vector minus the y component
        Vector3 vectorB = new Vector3(player.rigidBody.velocity.x, 0.0f, player.rigidBody.velocity.z);

        // Find the angle between vectorA and vectorB
        float rotateAngle = Vector3.SignedAngle(vectorA.normalized, vectorB.normalized, Vector3.up);

        // Get the target's speed (maginitude) without the y component
        // Only set speed factor when vector A and B are almost facing the same direction
        float speedFactor = Vector3.Dot(vectorA, vectorB) > 0.0f ? vectorB.magnitude : 1.0f;

        // Rotate towards the angle between vectorA and vectorB
        // Use speedFactor so camera doesn't rotatate at a constant speed
        // Limit speedFactor to be between 1 and 2
        transform.Rotate(Vector3.up, rotateAngle * Mathf.Clamp(speedFactor, 1.0f, 2.0f) * Time.deltaTime);


        // Attempt to not clip camera into environment
        //Debug.DrawRay(player.transform.position, mCamera.position - player.transform.position, Color.red);
        if (Physics.Raycast(player.transform.position, mCamera.position - player.transform.position, out RaycastHit hit, offset, whatIsGround))
        {
            float offsetChange = hit.distance;

            // Position the camera behind target at a distance of offset but not clipping environment
            transform.position = player.transform.position - (transform.forward * (offsetChange - 0.5f));
            transform.LookAt(player.transform.position);
        }
        else
        {
            // Position the camera behind target at a distance of offset
            transform.position = player.transform.position - (transform.forward * offset);
            transform.LookAt(player.transform.position);
        }

        
    }
}
