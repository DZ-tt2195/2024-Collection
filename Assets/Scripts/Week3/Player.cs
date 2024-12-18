using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace Week3
{
    public class Player : Moving
    {
        #region Setup

        [Foldout("Setup", true)]
        [SerializeField] RectTransform canvas;
        [SerializeField] Camera mainCam;

        [Foldout("Create Platforms", true)]
        List<SpriteRenderer> listOfPrefabs = new();
        int currentSelection = 0;
        int _platformsLeft;
        int PlatFormsLeft { get { return _platformsLeft; } set { platformCount.text = $"Platforms: {value}"; _platformsLeft = value; } }
        [SerializeField] TMP_Text platformCount;

        [Foldout("Jumping", true)]
        [SerializeField] float jumpHeight;
        float coyoteTime;
        GameObject wallJumpOff = null;
        Vector2 moveInput;
        NewControls controls;

        [Foldout("Misc", true)]
        Vector2 checkPoint;
        bool checkForScroll = true;

        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 0.75f;

            controls = new NewControls();
            controls.PlayerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.PlayerMovement.Move.canceled += ctx => moveInput = new(0, 0);
            controls.PlayerMovement.Jump.performed += ctx => Jump();
            controls.PlayerMovement.Scroll.performed += ctx => Scroll(ctx.ReadValue<Vector2>());
            controls.PlayerMovement.Place.performed += ctx => PlacePlatform();

            checkPoint = this.transform.position;
            applyForce.Add(PlayerMoveMe);

            PlatFormsLeft = 3;
            SpriteRenderer[] platformsToCreate = Resources.LoadAll<SpriteRenderer>("Week3/Platforms");
            foreach (SpriteRenderer next in platformsToCreate)
            {
                SpriteRenderer obj = Instantiate(next);
                obj.transform.SetParent(canvas, false);
                obj.gameObject.SetActive(false);
                listOfPrefabs.Add(obj);

                Collider[] colliders = obj.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    collider.enabled = false;
            }
        }

        void OnEnable()
        {
            controls.PlayerMovement.Enable();
        }

        void OnDisable()
        {
            controls.PlayerMovement.Disable();
        }

        #endregion

        #region Gameplay

        void Update()
        {
            SpriteRenderer currentPlatform = listOfPrefabs[currentSelection];
            currentPlatform.gameObject.SetActive(true);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            currentPlatform.transform.position = mouseWorldPosition;
            currentPlatform.SetAlpha(PlatFormsLeft > 0 ? 1 : 0.4f);

            coyoteTime = (cc.isGrounded) ? 0.15f : coyoteTime - Time.deltaTime;
        }

        private Vector2 PlayerMoveMe()
        {
            Vector2 movement = new(moveInput.x * moveSpeed, yMovement);
            return movement;
        }

        void Scroll(Vector2 scroll)
        {
            if (checkForScroll)
            {
                StartCoroutine(Resume());
                listOfPrefabs[currentSelection].gameObject.SetActive(false);
                if (scroll.y > 0)
                    currentSelection = (currentSelection + 1) % listOfPrefabs.Count;
                else
                    currentSelection = (currentSelection - 1 + listOfPrefabs.Count) % listOfPrefabs.Count;

                IEnumerator Resume()
                {
                    checkForScroll = false;
                    yield return new WaitForSeconds(0.1f);
                    checkForScroll = true;
                }
            }
        }

        void Jump()
        {
            if (wallJumpOff != null && !cc.isGrounded)
            {
                float pushEffect = ((wallJumpOff.transform.position.x < this.transform.position.x) ? moveSpeed : -moveSpeed)*1.5f;
                ChangeYMovement(jumpHeight*(0.75f));
                PushMe(new(pushEffect, 0), 0.33f);
            }
            else if (coyoteTime > 0f)
            {
                ChangeYMovement(jumpHeight);
            }
        }

        void PlacePlatform()
        {
            SpriteRenderer currentPlatform = listOfPrefabs[currentSelection];
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (PlatFormsLeft > 0 && !Physics.Raycast(ray, out RaycastHit hit) && !EventSystem.current.IsPointerOverGameObject())
            {
                PlatFormsLeft--;
                Vector3 worldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                SpriteRenderer newObject = Instantiate(currentPlatform, worldPos, Quaternion.identity);
                Collider[] colliders = newObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    collider.enabled = true;
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
            else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 collisionNormal = (transform.position - other.ClosestPoint(transform.position)).normalized;
                if (!cc.isGrounded && Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
                {
                    wallJumpOff = other.gameObject;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == wallJumpOff)
            {
                wallJumpOff = null;
            }
        }

        #endregion
    }
}