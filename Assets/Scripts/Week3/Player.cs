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
        [SerializeField] Image mouseSprite;
        [SerializeField] TMP_Text platformCount;

        [Foldout("Create Platforms", true)]
        SpriteRenderer platformToCreate;
        int numPlatform = 0;

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
            if (platformToCreate == null || numPlatform == 0)
            {
                mouseSprite.gameObject.SetActive(false);
                platformCount.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out Vector2 localPoint);
                mouseSprite.gameObject.SetActive(true);
                mouseSprite.transform.localPosition = localPoint;
                mouseSprite.sprite = platformToCreate.sprite;
                mouseSprite.transform.localScale = platformToCreate.transform.localScale;
                mouseSprite.color = platformCount.color;

                if (Input.GetMouseButtonDown(0))
                {
                    numPlatform--;
                    Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    worldPos.z = 0;
                    SpriteRenderer newObject = Instantiate(platformToCreate, worldPos, Quaternion.identity);
                    StartCoroutine(VanishingObject(newObject));

                    IEnumerator VanishingObject(SpriteRenderer newObject)
                    {
                        float maxTime = 4f;
                        float elapsedTime = maxTime;
                        while (elapsedTime > 0f)
                        {
                            elapsedTime -= Time.deltaTime;
                            ChangeAlpha(newObject, elapsedTime / maxTime);
                            yield return null;
                        }
                        Destroy(newObject.gameObject);
                    }
                }

                platformCount.transform.parent.gameObject.SetActive(true);
                platformCount.text = $"Platforms: {numPlatform}";
            }
        }

        void ChangeAlpha(SpriteRenderer sr, float alpha)
        {
            Color newColor = sr.color;
            newColor.a = alpha;
            sr.color = newColor;
        }

        private void FixedUpdate()
        {
            if (!cc.isGrounded)
                yMovement -= gravity;
            if (yMovement < -20)
                yMovement = -20;
            
            Vector3 movement = new(groundModify + (moveInput.x * moveSpeed), airModify + yMovement, 0f);
            cc.Move(movement*Time.deltaTime);
        }

        void Jump()
        {
            if (cc.isGrounded)
                yMovement = jumpHeight;
        }

        public void NewPlatforms(SpriteRenderer sr, int amount)
        {
            platformToCreate = sr;
            numPlatform = amount;
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