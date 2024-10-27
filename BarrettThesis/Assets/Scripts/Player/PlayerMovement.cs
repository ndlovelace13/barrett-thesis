using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float playerSpeed;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public Transform direction;

    public float playerHeight;
    public LayerMask ground;

    //states
    public bool grounded;
    public float groundDrag;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        GetInput();
        SpeedLimit();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        moveDirection = direction.forward * verticalInput + direction.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * playerSpeed * 10, ForceMode.Force);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void SpeedLimit()
    {
        Vector3 currentSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (currentSpeed.magnitude > playerSpeed)
        {
            Vector3 limitedSpeed = currentSpeed.normalized * playerSpeed;
            rb.velocity = new Vector3(limitedSpeed.x, rb.velocity.y, limitedSpeed.z);
        }
    }
}
