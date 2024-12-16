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
        [Foldout("Setup", true)]
        [SerializeField] RectTransform canvas;
        [SerializeField] Camera mainCam;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] Transform groundCheckPoint;

        [Foldout("UI", true)]
        [SerializeField] Image mouseSprite;
        [SerializeField] GameObject objectToInstantiate;

        [Foldout("Physics", true)]
        Rigidbody2D rb;
        float groundCheckRadius = 0.2f;
        [SerializeField] float moveSpeed;
        [SerializeField] float jumpHeight;
        [SerializeField] float gravity;
        [ReadOnly] public float moveModify = 0f;
        private bool isGrounded;
        public Vector2 moveInput { get; private set; }
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

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out Vector2 localPoint);
            mouseSprite.transform.localPosition = localPoint;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                if (hit.collider == null)
                    Instantiate(objectToInstantiate, worldPos, Quaternion.identity);
                else
                    Debug.LogError($"can't make object at {worldPos}");
            }
        }

        private void FixedUpdate()
        {
            if (!isGrounded)
                yMovement -= gravity;
            if (yMovement < -20)
                yMovement = -20;

            rb.velocity = new Vector2(moveModify + (moveInput.x * moveSpeed), yMovement);
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