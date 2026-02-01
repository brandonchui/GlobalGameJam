using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour {
    public GameObject hitParticlesPrefab;
    public Rigidbody2D rigid { get;  private set; }
    Collider2D col;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 50f;
    public float airControlMultiplier = 0.8f;

    [Header("Jump Settings")]
    public float jumpPower = 10.0f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float variableJumpMultiplier = 0.5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // Jump state
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private bool jumpHeld = false;

    Vector2 myLook;
    Vector2 lookDirection;

    public DebugCircle myCircle;
    [HideInInspector] public DebugCircle secondCircle;
    [HideInInspector] public bool singlePlayerMode = false;
    private bool circleToggle = false;

    SpriteRenderer sr;
    public Sprite idleSprite;
    public Sprite jumpSprite;
    public Sprite[] runSprites;

    private enum AnimState {
        IDLE,
        JUMP,
        RUN
    }
    bool facing = false;
    float runTimer = 0.0f;

    AnimState animState;

    //float timeSinceGrounded = 5.0f;
    bool grounded = false;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMovePerformed;
        playerInput.actions["Move"].canceled += OnMoveCanceled;
        playerInput.actions["Jump"].performed += OnJumpPerformed;
        playerInput.actions["Jump"].canceled += OnJumpCanceled;
        playerInput.actions["Look"].performed += OnLookPerformed;
        playerInput.actions["Look"].canceled += OnLookCanceled;
    }

    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMovePerformed;
        playerInput.actions["Move"].canceled -= OnMoveCanceled;
        playerInput.actions["Jump"].performed -= OnJumpPerformed;
        playerInput.actions["Jump"].canceled -= OnJumpCanceled;
        playerInput.actions["Look"].performed -= OnLookPerformed;
        playerInput.actions["Look"].canceled -= OnLookCanceled;
    }

    private void OnLookPerformed(InputAction.CallbackContext context) {
        lookDirection = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context) {
        lookDirection = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context) {
        jumpHeld = true;
        jumpBufferTimer = jumpBufferTime;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context) {
        jumpHeld = false;
    }

    private void TryJump() {
        if (controlsActive && (grounded || coyoteTimer > 0f)) {
            Vector3 v = rigid.linearVelocity;
            v.y = jumpPower;
            rigid.linearVelocity = v;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context) {
        moveInput = Vector2.zero;
    }

    bool controlsActive = true;
    float freezeTimer = 0f;
    public void FreezeControls(float time) {
        freezeTimer = time;
        controlsActive = false;
    }

    private void Update() {
        freezeTimer -= Time.deltaTime;
        controlsActive = freezeTimer < 0.0f;

        // Single player: toggle between circles with left click
        if (singlePlayerMode && secondCircle != null) {
            var mouse = Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame) {
                circleToggle = !circleToggle;
                // Swap z-order to show active circle on top
                if (myCircle) myCircle.transform.position = new Vector3(myCircle.transform.position.x, myCircle.transform.position.y, circleToggle ? -1 : -2);
                if (secondCircle) secondCircle.transform.position = new Vector3(secondCircle.transform.position.x, secondCircle.transform.position.y, circleToggle ? -2 : -1);
            }
        }

        // Move active circle
        DebugCircle activeCircle = (singlePlayerMode && circleToggle) ? secondCircle : myCircle;
        if (activeCircle) {
            if (playerInput.currentControlScheme == "Keyboard&Mouse") {
                var mouse = Mouse.current;
                myLook = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            }
            else {
                // Sync myLook with actual circle position (in case it was clamped)
                myLook = new Vector2(activeCircle.transform.position.x, activeCircle.transform.position.y);
                myLook += lookDirection * 0.075f;
            }
            activeCircle.SetPosition(new Vector3(myLook.x, myLook.y, activeCircle.transform.position.z));
        }

        // Coyote time
        if (grounded) {
            coyoteTimer = coyoteTime;
        } else {
            coyoteTimer -= Time.deltaTime;
        }

        // Jump buffer
        if (jumpBufferTimer > 0f) {
            jumpBufferTimer -= Time.deltaTime;
            TryJump();
        }

        // Horizontal movement with acceleration
        if (controlsActive) {
            float targetSpeed = moveInput.x * moveSpeed;
            float currentSpeed = rigid.linearVelocity.x;
            float accelRate = grounded ?
                (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration) :
                (Mathf.Abs(targetSpeed) > 0.01f ? acceleration * airControlMultiplier : deceleration * airControlMultiplier);

            float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.deltaTime);
            rigid.linearVelocity = new Vector2(newSpeed, rigid.linearVelocity.y);
        }

        // Variable jump height & fall multiplier (only when not in knockback)
        if (rigid.linearVelocity.y < 0) {
            // Falling - apply fall multiplier
            //rigid.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rigid.linearVelocity.y > 0 && !jumpHeld && controlsActive) {
            // Rising but jump released - cut jump short (skip during knockback)
            rigid.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (grounded) {
            if (moveInput.sqrMagnitude > 0.01) {
                if(animState != AnimState.RUN) {
                    runTimer = 0.0f;
                }
                else {
                    runTimer += Time.deltaTime;
                }
                animState = AnimState.RUN;
            }
            else {
                animState = AnimState.IDLE;
            }
        }
        else {
            animState = AnimState.JUMP;
        }
        if (moveInput.x != 0.0f) {
            facing = moveInput.x < 0.0f;
        }

        switch (animState) {
            case AnimState.IDLE:
                sr.sprite = idleSprite;
                break;
            case AnimState.JUMP:
                sr.sprite = jumpSprite;
                break;
            case AnimState.RUN:
                int i = (int)(runTimer / 0.1f) % runSprites.Length;
                sr.sprite = runSprites[i];
                break;
        }
        Vector3 scale = sr.transform.localScale;
        scale.x = facing ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        sr.transform.localScale = scale;

        var hits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size * 0.95f, 0, Vector2.down, 0.2f, groundMask);
        grounded = false;
        for (int i = 0; i < hits.Length; i++) {
            var hit = hits[i];
            if(hit.collider != col && hit.normal.y > 0.5f) {
                grounded = true;
                break;
            }
        }
        //grounded = hits.Length > 0;
        //timeSinceGrounded = hits.Length > 0 ? 0.0f : timeSinceGrounded + Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Platform")) {
            var contact = collision.GetContact(0);
            Instantiate(hitParticlesPrefab, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
        }
    }
}
