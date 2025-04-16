using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Component references
    private new Camera camera;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    // Input & physics
    private Vector2 velocity;
    private float inputAxis;
    private float accelerationTime; // Time holding direction

    [Header("Movement Settings")]
    public float moveSpeed = 8f;            // Maximum move speed
    public float baseAcceleration = 30f;    // Base acceleration when just starting
    public float maxAcceleration = 80f;     // Max acceleration when holding input
    public float accelerationRampTime = 1f; // Time to reach max acceleration
    public float airControlFactor = 0.66f;   // Reduced control in air

    [Header("Jump Settings")]
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpBufferTime = 0.1f;
    public float coyoteTime = 0.1f;

    private float jumpBufferCounter;
    private float coyoteCounter;

    // Derived physics values
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    // State booleans
    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.35f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0.15f) || (inputAxis < 0f && velocity.x > 0.15f);

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        camera = Camera.main;
    }

    private void OnEnable()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable()
    {
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        HandleHorizontalMovement();

        // Ground check
        bool wasGrounded = grounded;
        grounded = rigidbody.Raycast(Vector2.down);

        // Coyote time timer
        if (grounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        // Jump buffer
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Perform jump
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            velocity.y = jumpForce;
            jumping = true;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if (grounded)
{
    velocity.y = Mathf.Max(velocity.y, 0f);

    // Only reset jumping if we've actually landed (not just touching ground during jump start)
    if (Mathf.Abs(velocity.y) < 0.01f)
    {
        jumping = false;
        accelerationTime = 0f;
    }
}

        ApplyGravity();
    }

    private void HandleHorizontalMovement()
    {
        inputAxis = Input.GetAxisRaw("Horizontal");

        // If holding a direction, ramp up acceleration time
        if (Mathf.Abs(inputAxis) > 0.1f)
            accelerationTime += Time.deltaTime;
        else
            accelerationTime = 0f;

        // Calculate dynamic acceleration based on hold time (ease-in curve)
        float accelerationPercent = Mathf.Clamp01(accelerationTime / accelerationRampTime);
        float currentAcceleration = Mathf.Lerp(baseAcceleration, maxAcceleration, accelerationPercent);

        // Reduce control if in the air
        float effectiveAcceleration = grounded ? currentAcceleration : currentAcceleration * airControlFactor;

        // Target speed and velocity update
        float targetSpeed = inputAxis * moveSpeed;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, effectiveAcceleration * Time.deltaTime);

        // Stop on wall
        if (rigidbody.Raycast(Vector2.right * Mathf.Sign(velocity.x)))
        {
            velocity.x = 0f;
        }

        // Flip sprite
        if (velocity.x > 0f)
            transform.eulerAngles = Vector3.zero;
        else if (velocity.x < 0f)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0f;
        bool jumpCut = !Input.GetButton("Jump") && velocity.y > 0f;

        float multiplier = falling ? 2f : (jumpCut ? 2f : 1f);
        velocity.y += gravity * multiplier * Time.deltaTime;

        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void FixedUpdate()
    {
        // Apply final movement
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        // Clamp horizontal movement to screen
        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        rigidbody.MovePosition(position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bounce on enemies
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (transform.DotTest(collision.transform, Vector2.down))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }

        // Hit ceiling
        if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            if (transform.DotTest(collision.transform, Vector2.up))
            {
                velocity.y = 0f;
            }
        }
    }
}

/* ----- OLD VERSION
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private new Camera camera;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 10f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    public bool grounded {get; private set;}
    public bool jumping {get; private set;}
    public bool running => Mathf.Abs(velocity.x) > 0.35f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0.15f) || (inputAxis < 0f && velocity.x > 0.15f);



// slide için runningde ayarlamalar yapabilirim running statementi true daha fazla neyse bakcam

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        camera = Camera.main;
    }

    private void OnEnable()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        velocity= Vector2.zero;
        jumping = false;
        
    }

    private void OnDisable()
    {
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {

        HorizontalMovement();

        grounded = rigidbody.Raycast(Vector2.down);

        if(grounded){
            GroundedMovement();
        }
        
        ApplyGravity();
    }


    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed* Time.deltaTime);

        if(rigidbody.Raycast(Vector2.right * velocity.x)){
            velocity.x = 0f;
        }
        if(velocity.x > 0){
            transform.eulerAngles = Vector3.zero;
        }else if(velocity.x < 0f){
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
    private void GroundedMovement()
    {
        
        velocity.y = Mathf.Max(velocity.y, 0f); //to prevent overgravity force stack
        jumping = velocity.y > 0f;


        if(Input.GetButtonDown("Jump")){
            velocity.y = jumpForce;
            jumping =  true;
        }
    }


    private void ApplyGravity()
    {
        bool falling  = velocity.y < 0f || !Input.GetButton("Jump"); //when you let go the input the higher jump will be disabled
        float multiplier = falling ? 2f : 1f;
        velocity.y += gravity * multiplier * Time.deltaTime;

        velocity.y = Mathf.Max(velocity.y, gravity / 2f);

    }
    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);
        rigidbody.MovePosition(position);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            if(transform.DotTest(collision.transform, Vector2.down)){
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        if(collision.gameObject.layer != LayerMask.NameToLayer("PowerUp")){
            if(transform.DotTest(collision.transform, Vector2.up)){
                velocity.y = 0f;
            }
                }
    }

}





*/