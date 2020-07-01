using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State //add states as needed
    {
        NORMAL
    }

    [HideInInspector]
    public State currentState;

    [HideInInspector]
    public Rigidbody rigidBody;

    [HideInInspector]
    public Vector3 floorNormal;

    [HideInInspector]
    public bool overRamp = false;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private float groundCheckRadius = 0.25f;

    [SerializeField]
    public float maxSpeed = 15;

    [SerializeField]
    private float moveForce = 5;
    

    

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  // Get rigidbody component

        //currentState = State.WAIT;            // Set current state to WAIT
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position - (Vector3.up * 0.5f), groundCheckRadius);
    //}

    /// <summary>
    /// Applies force to player object
    /// </summary>
    /// <param name="verticalTilt">Scales force applied in the forward direction (Ranges between 1 and -1)</param>
    /// <param name="horizontalTilt">Scales force applied in the horizontal direction (Ranges between 1 and -1)</param>
    /// <param name="right">The horizontal direction</param>
    public void Move(float verticalTilt, float horizontalTilt, Vector3 right)
    {
        // Only apply movement when the player is grounded
        if (OnGround())
        {
            CalculateFloorNormal();

            // No input from player
            if (horizontalTilt == 0.0f && verticalTilt == 0.0f && rigidBody.velocity.magnitude > 0.0f)
            {
                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, moveForce * 0.1f * Time.deltaTime); // Slow down
            }
            else if (rigidBody.velocity.magnitude < maxSpeed) //restrict speed
            {
                // Get a direction perpendicular to the camera's right vector and the floor's normal (The forward direction)
                Vector3 forward = Vector3.Cross(right, floorNormal);

                // Apply moveForce scaled by verticalTilt in the forward direction (Half the move force when moving backwards)
                Vector3 forwardForce = (verticalTilt > 0.0f ? 1.0f : 0.5f) * moveForce * verticalTilt * forward;
                // Apply moveForce scaled by horizontalTilt in the right direction
                Vector3 rightForce = moveForce * horizontalTilt * right;

                Vector3 forceVector = forwardForce + rightForce;

                rigidBody.AddForce(forceVector);
            }
        }


        //Check if we need to offset camera because of ramp
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        //Debug.DrawRay(transform.position, Vector3.down, Color.red);

        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Ramp"))
        {
            overRamp = true;
        }
        else overRamp = false;
    }

    /// <summary>
    /// Checks if the player is grounded (Above an object within whatIsGround layer mask)
    /// </summary>
    /// <returns>
    /// True if CheckSphere overlaps with collider within the whatIsGround layer mask, else return False
    /// </returns>
    /// 
    public bool OnGround() 
    {
        
        return Physics.CheckSphere(transform.position - (Vector3.up * 0.5f), groundCheckRadius, whatIsGround);
    }

    

    /// <summary>
    /// Sets floor normal by casting ray below player
    /// </summary>
    private void CalculateFloorNormal()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, whatIsGround))
        {
            floorNormal = hit.normal;
        }
    }
}
