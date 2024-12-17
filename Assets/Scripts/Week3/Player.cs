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
    public class Player : Moving
    {
        [Foldout("Setup", true)]
        [SerializeField] RectTransform canvas;
        [SerializeField] Camera mainCam;
        [SerializeField] Image mouseSprite;

        [Foldout("Create Platforms", true)]
        SpriteRenderer[] platformsToCreate;
        int currentSelection = 0;
        int _platformsLeft;
        int PlatFormsLeft { get { return _platformsLeft; } set { platformCount.text = $"Platforms: {value}"; _platformsLeft = value; } }
        [SerializeField] TMP_Text platformCount;

        [Foldout("Misc", true)]
        Vector2 checkPoint;
        Vector2 moveInput;
        NewControls controls;
        [SerializeField] float jumpHeight;

        protected override void Awake()
        {
            base.Awake();
            controls = new NewControls();
            checkPoint = this.transform.position;

            PlatFormsLeft = 3;
            platformsToCreate = Resources.LoadAll<SpriteRenderer>("Week3/Platforms");

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
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.5f)
            {
                if (Input.mouseScrollDelta.y < 0)
                    currentSelection = (currentSelection - 1 + platformsToCreate.Length) % platformsToCreate.Length;
                else if (Input.mouseScrollDelta.y > 0)
                    currentSelection = (currentSelection + 1) % platformsToCreate.Length;
            }

            SpriteRenderer currentPlatform = platformsToCreate[currentSelection];
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out Vector2 localPoint);
            mouseSprite.transform.localPosition = localPoint;
            mouseSprite.sprite = currentPlatform.sprite;
            mouseSprite.transform.localScale = currentPlatform.transform.localScale;
            mouseSprite.color = currentPlatform.color;
            mouseSprite.SetAlpha(PlatFormsLeft > 0 ? 1 : 0.4f);

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && PlatFormsLeft > 0 && !Physics.Raycast(ray, out RaycastHit hit))
            {
                PlatFormsLeft--;
                Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;
                SpriteRenderer newObject = Instantiate(currentPlatform, worldPos, Quaternion.identity);
                StartCoroutine(VanishingObject(newObject));

                IEnumerator VanishingObject(SpriteRenderer newObject)
                {
                    float maxTime = 3f;
                    float elapsedTime = maxTime;
                    while (elapsedTime > 0f)
                    {
                        elapsedTime -= Time.deltaTime;
                        ChangeAlpha(newObject, elapsedTime / maxTime);
                        yield return null;
                    }
                    PlatFormsLeft++;
                    Destroy(newObject.gameObject);
                }
            }
        }

        void Jump()
        {
            if (cc.isGrounded)
            {
                ChangeYMovement(jumpHeight);
            }
        }

        protected override Vector2 MoveMe()
        {
            Vector2 movement = new(groundModify + (moveInput.x * moveSpeed), yMovement);
            return movement;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Checkpoint"))
            {
                this.checkPoint = other.transform.position;
            }
            else if (other.CompareTag("Death"))
            {
                this.transform.position = this.checkPoint;
            }
        }
    }
}