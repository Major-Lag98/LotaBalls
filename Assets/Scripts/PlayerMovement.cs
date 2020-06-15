using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveX = new Vector3();
        Vector3 moveY = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            moveY = Camera.main.transform.forward;//Input.GetAxis("Horizontal");
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -Camera.main.transform.forward;//Input.GetAxis("Vertical");
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -Camera.main.transform.right;//Input.GetAxis("Vertical");
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = Camera.main.transform.right;
        }
         

        Vector3 movement = moveX + moveY;

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(-rb.velocity);
        }
        else
            rb.AddForce(movement);

        
    }
}
