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
        CharacterController cc;
        public Vector2 moveInput { get; private set; }
        [SerializeField] float moveSpeed;
        float groundModify = 0f;
        [SerializeField] float jumpHeight;
        [SerializeField] float gravity;
        float airModify = 0f;
        float yMovement = 0;
        NewControls controls;

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
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
            if (!cc.isGrounded)
                yMovement -= gravity;
            if (yMovement < -15)
                yMovement = -15;

            Vector3 movement = new(groundModify + (moveInput.x * moveSpeed), airModify + yMovement, 0f);
            cc.Move(movement*Time.deltaTime);
        }

        void Jump()
        {
            if (cc.isGrounded)
                yMovement = jumpHeight;
        }

        public void ModifyGroundSpeed(float speed)
        {
            groundModify += speed;
        }

        public void ModifyAirSpeed(float speed)
        {
            airModify += speed;
        }
    }
}