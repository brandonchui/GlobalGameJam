using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour {
    public Rigidbody2D rigid;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public LayerMask groundMask;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpPower = 10.0f;
    bool grounded = false;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
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

        Vector3 v = rigid.linearVelocity;
        v.x = moveInput.x * moveSpeed;

        rigid.linearVelocity = v;

        var hits = Physics2D.BoxCastAll(transform.position, Vector2.one, 0, Vector2.down, -1.0f, groundMask);
        grounded = hits.Length > 0;
        Debug.Log(grounded);
    }
}
