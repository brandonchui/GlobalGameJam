using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour {
    public ParticleSystem hairParticles;
    public GameObject hitParticlesPrefab;
    public Rigidbody2D rigid { get; private set; }
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

    [Header("Lift Up Co-op Settings")]
    public float liftGravityScale = 0.5f;  // Half gravity
    public ParticleSystem liftParticles;

    // Jump state
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private bool jumpHeld = false;

    // Lift up state
    bool tryToLift = false;

    Vector2 myLook;
    Vector2 lookDirection;

    public DebugCircle myCircle;
    [HideInInspector] public DebugCircle secondCircle;
    [HideInInspector] public CharacterController2D otherBoy;
    [HideInInspector] public bool singlePlayerMode = false;
    private bool circleToggle = false;

    SpriteRenderer sr;
    public Sprite idleSprite;
    public Sprite liftSprite;
    public Sprite jumpSprite;
    public Sprite[] runSprites;

    Light2D myLight;

    private enum AnimState {
        IDLE,
        JUMP,
        RUN
    }
    bool facing = false;
    float runTimer = 0.0f;

    AnimState animState;

    bool grounded = false;

    [HideInInspector] public List<Color> colors = new List<Color>();

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
        sr = GetComponentInChildren<SpriteRenderer>();
        myLight = GetComponentInChildren<Light2D>();

        colors.Add(sr.color);
        colors.Add(myLight.color);

        var main = hairParticles.main;
        colors.Add(main.startColor.color);
    }

    void SetColors(List<Color> cols) {
        sr.color = cols[0];
        myLight.color = cols[1];
        var main = hairParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(cols[2]);
    }


    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMovePerformed;
        playerInput.actions["Move"].canceled += OnMoveCanceled;
        playerInput.actions["Jump"].performed += OnJumpPerformed;
        playerInput.actions["Jump"].canceled += OnJumpCanceled;
        playerInput.actions["Look"].performed += OnLookPerformed;
        playerInput.actions["Look"].canceled += OnLookCanceled;
        playerInput.actions["Attack"].performed += OnAttackPerformed;
        playerInput.actions["Attack"].canceled += OnAttackCanceled;
    }


    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMovePerformed;
        playerInput.actions["Move"].canceled -= OnMoveCanceled;
        playerInput.actions["Jump"].performed -= OnJumpPerformed;
        playerInput.actions["Jump"].canceled -= OnJumpCanceled;
        playerInput.actions["Look"].performed -= OnLookPerformed;
        playerInput.actions["Look"].canceled -= OnLookCanceled;
        playerInput.actions["Attack"].performed -= OnAttackPerformed;
        playerInput.actions["Attack"].canceled -= OnAttackCanceled;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context) {
        tryToLift = true;
        if (CanLift()) {
            AudioManager.Instance.PlayLift(1.5f);
        }
    }

    private void OnAttackCanceled(InputAction.CallbackContext context) {
        tryToLift = false;
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

            AudioManager.Instance.PlayJump(0.8f);
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
                SetColors(!circleToggle ? colors : otherBoy.colors);
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
        }
        else {
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
        }
        else if (rigid.linearVelocity.y > 0 && !jumpHeld && controlsActive) {
            // Rising but jump released - cut jump short (skip during knockback)
            rigid.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (grounded) {
            if (moveInput.sqrMagnitude > 0.01) {
                if (animState != AnimState.RUN) {
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
                sr.sprite = tryToLift ? liftSprite : idleSprite;
                Vector3 p = hairParticles.transform.localPosition;
                p.y = tryToLift ? -0.15f : 0.0f;
                hairParticles.transform.localPosition = p;
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
            if (hit.collider != col && hit.normal.y > 0.5f) {
                grounded = true;
                break;
            }
        }

        if (GameManager.IsCoop) {
            // find lower player
            var p1 = GameManager.Instance.player1;
            var p2 = GameManager.Instance.player2;

            var lower = p1.transform.position.y < p2.transform.position.y ? p1 : p2;
            var higher = p1.transform.position.y < p2.transform.position.y ? p2 : p1;
            if (lower == this) { // cant lift if youre the lower guy
                higher.SetLiftState(false);
            }
            else {
                float posDiff = Mathf.Abs(p1.transform.position.y - p2.transform.position.y);
                lower.SetLiftState(tryToLift && CanLift() && posDiff > 0.1f);
            }
        }

    }

    public bool CanLift() {
        return grounded && Mathf.Abs(moveInput.x) < 0.001f;
    }

    public void SetLiftState(bool lifted) {
        rigid.gravityScale = lifted ? 0.5f : 1.0f;
        if (liftParticles) {
            if (lifted && !liftParticles.isPlaying) {
                liftParticles.Play();
            }
            var em = liftParticles.emission;
            em.enabled = lifted;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        bool isPlatform = collision.gameObject.CompareTag("Platform");
        bool isPlatformWood = collision.gameObject.CompareTag("PlatformWood");
        if (isPlatform || isPlatformWood) {
            var contact = collision.GetContact(0);
            if (!isPlatformWood || contact.normal.y > -0.5f) {
                //tried to make particles spawn on edge of player but looks worse actually most of the time
                //var c = col.bounds.center;
                //c -= new Vector3(contact.normal.x * col.bounds.extents.x, contact.normal.y * col.bounds.extents.y, 0.0f);
                Instantiate(hitParticlesPrefab, contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        if (!GameManager.IsCoop) return;

        var players = FindObjectsByType<CharacterController2D>(FindObjectsSortMode.None);
        if (players.Length < 2) return;

        // Find lowest player (the one who can be boosted)
        CharacterController2D lowest = null;
        float lowestY = float.MaxValue;
        foreach (var player in players) {
            if (player.transform.position.y < lowestY) {
                lowestY = player.transform.position.y;
                lowest = player;
            }
        }

        // Draw circle above lowest player (they can be boosted)
        if (lowest == this) {
            Gizmos.color = Color.yellow;
            Vector3 circlePos = transform.position + Vector3.up * 1.5f;
            Gizmos.DrawWireSphere(circlePos, 0.3f);
        }
    }
#endif
}
