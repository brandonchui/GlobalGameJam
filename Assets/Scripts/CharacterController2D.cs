using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour {
    public Rigidbody2D rigid { get;  private set; }
    Collider2D col;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpPower = 10.0f;
    Vector2 myLook;
    Vector2 lookDirection;

    public DebugCircle myCircle;

    //float timeSinceGrounded = 5.0f;
    bool grounded = false;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMovePerformed;
        playerInput.actions["Move"].canceled += OnMoveCanceled;
        playerInput.actions["Jump"].performed += OnJumpPerformed;
        playerInput.actions["Look"].performed += OnLookPerformed;
        playerInput.actions["Look"].canceled += OnLookCanceled;
    }

    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMovePerformed;
        playerInput.actions["Move"].canceled -= OnMoveCanceled;
        playerInput.actions["Jump"].performed -= OnJumpPerformed;
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
        if (grounded && controlsActive) {
            Vector3 v = rigid.linearVelocity;
            v.y = jumpPower;
            rigid.linearVelocity = v;
            grounded = false;
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

        Vector3 cam = Camera.main.transform.position;
        Vector3 me = transform.position;
        cam = new Vector3(me.x, me.y, cam.z);
        Camera.main.transform.position = cam;

        if (myCircle) {
            if (playerInput.currentControlScheme == "Keyboard&Mouse") { // skip this shit if on keyb
                var mouse = Mouse.current;
                myLook = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            }
            else {
                myLook += lookDirection * 0.075f;
            }
            //Vector3 pos = Camera.main.ScreenToWorldPoint(myLook);
            myCircle.SetPosition(new Vector3(myLook.x, myLook.y, -2));
        }

        if (controlsActive) {
            Vector3 v = rigid.linearVelocity;
            v.x = moveInput.x * moveSpeed;
            rigid.linearVelocity = v;
        }

        var hits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size * 0.95f, 0, Vector2.down, 0.2f, groundMask);
        grounded = false;
        for (int i = 0; i < hits.Length; i++) {
            var hit = hits[i];
            if(hit.collider != col) {
                grounded = true;
                break;
            }
        }
        //grounded = hits.Length > 0;
        //timeSinceGrounded = hits.Length > 0 ? 0.0f : timeSinceGrounded + Time.deltaTime;
    }
}
