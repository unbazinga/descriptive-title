using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Generic Variables")] 
    public Animator playerAnimator;

    public string currentAnim = "Idle";
    public Transform playerCam;
    public Transform orientation;
    public Transform objectInteract;
    public LayerMask objectInteractLayers;
    public static GameObject holdArea;
    public WeaponInterface weaponInterface;
    [FormerlySerializedAs("ObjectInteractReach")] public float objectInteractReach;
    private Rigidbody rb;
    private bool isInteractableHighlighted = false;
    
    [Header("Movement -- Generic")]
    public float moveSpeed;

    public float defMoveSpeed;
    public float runMulti;
    public bool grounded;
    public float maxSpeed;
    public float groundDrag;
    public float playerHeight;
    public LayerMask whatIsGround;
    private Vector3 moveDir;
    
    [Header("Movement -- Jumping")]
    private bool readyToJump = true;
    private float jumpCooldown = .75f;
    private float timeToJump = 0.15f;
    private float waitBeforeJump = 0f;
    public float jumpForce;
    private float airMoveMulti = 0.25f;

    [Header("Weapon Stuff")] 
    public WeaponAttachmentDefinition[] HeldAttachments;


    
    // Input
    private float x, y;
    private bool jumping, crouching;
    public bool sprinting;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var bruh in WeaponDefinition.GetEnumList<WeaponDefinition.WeaponFireMode>())
        {
            // Should log each value in WeaponFireMode
            Debug.Log(bruh);
        }
        Debug.Log(WeaponDefinition.GetEnumList<WeaponDefinition.WeaponFireMode>());
        holdArea = GameObject.FindGameObjectWithTag("Holder");
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position + new Vector3(0,0.05f,0), Vector3.down, playerHeight / 3.2f, whatIsGround);
        
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
        
        waitBeforeJump += Time.deltaTime;
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
        waitBeforeJump = 0f;
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

        if (Input.GetButtonDown("Jump") && readyToJump && grounded && waitBeforeJump > timeToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (Input.GetButtonDown("Sprint") && grounded && Math.Abs(moveSpeed - defMoveSpeed) < 0.05f)
        {
            moveSpeed *= runMulti;
            sprinting = true;
        } else if (Input.GetButtonUp("Sprint") && Math.Abs(moveSpeed - defMoveSpeed) > 0.05f)
        {
            moveSpeed = defMoveSpeed;
            sprinting = false;
        }
        if(grounded) {        
            if (Input.GetKey(KeyCode.S))
            {
                currentAnim = "Walking Backwards";
                // playerAnimator.CrossFadeInFixedTime("Walking Backwards", 0.1f);
                // playerAnimator.Play("Walking Backwards");
            } else if (Input.GetKey(KeyCode.W))
            {
                // if (Input.GetKey(KeyCode.D))
                // {
                //     currentAnim = "Forwards Right";
                //     // playerAnimator.CrossFadeInFixedTime("Forwards Right", 0.1f);
                //     // playerAnimator.Play("Forwards Right");
                //
                // }
                // else
                // {

                    currentAnim = "Walking Forwards";
                    // playerAnimator.CrossFadeInFixedTime("Walking Forwards", 0.1f);
                    // playerAnimator.Play("Walking Forwards");
    
                // }
            } else if (Input.GetKey(KeyCode.Space))
            {
                currentAnim = "Jump";
            }
            else
            {
                
                currentAnim = "Idle";
                // playerAnimator.CrossFadeInFixedTime("Idle", 0.1f);
                // playerAnimator.Play("Idle");
    
            }
        }

        //playerAnimator.CrossFade(currentAnim, 0.25f);
        playerAnimator.Play(currentAnim);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + new Vector3(0,0.05f,0), transform.TransformDirection(Vector3.down.normalized * (playerHeight / 5.6f)));
    }
}
