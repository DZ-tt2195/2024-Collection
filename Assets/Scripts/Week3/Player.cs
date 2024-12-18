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

        [Foldout("Create Platforms", true)]
        List<SpriteRenderer> listOfPrefabs = new();
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

            controls.PlayerMovement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.PlayerMovement.Move.canceled += ctx => moveInput = new(0, 0);
            controls.PlayerMovement.Jump.performed += ctx => Jump();
            controls.PlayerMovement.Scroll.performed += ctx => Scroll(ctx.ReadValue<Vector2>());
            controls.PlayerMovement.Place.performed += ctx => PlacePlatform();
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
            SpriteRenderer currentPlatform = listOfPrefabs[currentSelection];
            currentPlatform.gameObject.SetActive(true);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            currentPlatform.transform.position = mouseWorldPosition;
            currentPlatform.SetAlpha(PlatFormsLeft > 0 ? 1 : 0.4f);
        }

        void Scroll(Vector2 scroll)
        {
            listOfPrefabs[currentSelection].gameObject.SetActive(false);
            if (scroll.y > 0)
                currentSelection = (currentSelection + 1) % listOfPrefabs.Count;
            else
                currentSelection = (currentSelection - 1 + listOfPrefabs.Count) % listOfPrefabs.Count;
        }

        void Jump()
        {
            if (cc.isGrounded)
            {
                ChangeYMovement(jumpHeight);
            }
        }

        void PlacePlatform()
        {
            SpriteRenderer currentPlatform = listOfPrefabs[currentSelection];
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (PlatFormsLeft > 0 && !Physics.Raycast(ray, out RaycastHit hit))
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