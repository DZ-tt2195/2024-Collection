using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.InputSystem;

namespace Week3
{
    public class Player : MonoBehaviour
    {
        Rigidbody2D rb;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] Transform groundCheckPoint;
        float groundCheckRadius = 0.2f;

        [SerializeField] float moveSpeed;
        [SerializeField] float jumpHeight;
        [SerializeField] float gravity;

        private bool isGrounded;
        private Vector2 moveInput;
        float yMovement = 0;
        private NewControls controls;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            groundCheckPoint.gameObject.SetActive(false);
            controls = new NewControls();

            controls.PlayerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.PlayerMovement.Move.canceled += ctx => moveInput = new(0, 0);
            controls.PlayerMovement.Jump.performed += ctx => Jump();
        }

        void OnEnable()
        {
            controls.PlayerMovement.Enable();
        }

        void OnDisable()
        {
            controls.PlayerMovement.Disable();
        }

        void Update()
        {
            if (yMovement <= 0)
                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
            else
                isGrounded = false;
        }

        private void FixedUpdate()
        {
            if (!isGrounded)
                yMovement -= gravity;
            rb.velocity = new Vector2(moveInput.x * moveSpeed, yMovement);
        }

        void Jump()
        {
            if (isGrounded)
            {
                isGrounded = false;
                yMovement = jumpHeight;
            }
        }
    }
}