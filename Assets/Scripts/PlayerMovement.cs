using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Generic Variables")]
    public Transform playerCam;
    public Transform orientation;
    private Rigidbody rb;
    
    [Header("Movement -- Generic")]
    public float moveSpeed;
    public float runMulti;
    public bool grounded;
    public float maxSpeed;
    public float groundDrag;
    public float playerHeight;
    public LayerMask whatIsGround;
    private Vector3 moveDir;
    
    [Header("Movement -- Jumping")]
    private bool readyToJump = true;
    private float jumpCooldown = 0.45f;
    public float jumpForce;
    private float airMoveMulti = 0.25f;

    [Header("Weapon Stuff")]


    // Input
    private float x, y;
    private bool jumping, sprinting, crouching;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        Inputs();
        SpeedControls();
        
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        moveDir = orientation.forward * y + orientation.right * x;
        if(grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMoveMulti, ForceMode.Force);
        
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }

    void Inputs()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            
        }
        if (Input.GetButtonDown("Fire2"))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            
        }
        

    }

    void SpeedControls()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    
    
}
