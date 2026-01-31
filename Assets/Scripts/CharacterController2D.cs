using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour {
    Rigidbody2D rigid;
    Collider2D col;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpPower = 10.0f;

    public DebugCircle circle1;
    public DebugCircle circle2;
    bool circleToggle = false;
    //float timeSinceGrounded = 5.0f;
    bool grounded = false;

    public TextMeshProUGUI heightText;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable() {
        playerInput.actions["Move"].performed += OnMovePerformed;
        playerInput.actions["Move"].canceled += OnMoveCanceled;
        playerInput.actions["Jump"].performed += OnJumpPerformed;
    }
    private void OnDisable() {
        playerInput.actions["Move"].performed -= OnMovePerformed;
        playerInput.actions["Move"].canceled -= OnMoveCanceled;
        playerInput.actions["Jump"].performed -= OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context) {
        if (grounded) {
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

    private void Update() {
        Vector3 cam = Camera.main.transform.position;
        Vector3 me = transform.position;
        cam = new Vector3(me.x, me.y, cam.z);
        Camera.main.transform.position = cam;

        heightText.SetText(me.y.ToString("F1") + "m");

        var mouse = Mouse.current;
        if (mouse != null) {
            if (circle1 && circle2) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
                var circ = circleToggle ? circle1 : circle2;
                circ.SetPosition(new Vector3(mousePos.x, mousePos.y, 0));
            }
            if (mouse.leftButton.wasPressedThisFrame) {
                circleToggle = !circleToggle;
            }
        }

        Vector3 v = rigid.linearVelocity;
        v.x = moveInput.x * moveSpeed;

        rigid.linearVelocity = v;

        var hits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size*0.95f, 0, Vector2.down, 0.2f, groundMask);
        grounded = hits.Length > 0;
        //timeSinceGrounded = hits.Length > 0 ? 0.0f : timeSinceGrounded + Time.deltaTime;
    }
}
