using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Week1
{
    public class Player : Entity
    {
        public static Player instance;
        public Camera mainCamera;

        [SerializeField] Slider healthSlider;
        [SerializeField] TMP_Text healthCounter;

        int currentBullet = 0;
        float currentRecharge = 0f;
        [SerializeField] Slider bulletSlider;
        [SerializeField] TMP_Text bulletCounter;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            this.Setup(3, 2f, "Player");
        }

        void Update()
        {
            if (health > 0)
            {
                FollowMouse();
                ShootBullet();
            }

            healthSlider.value = health / 3f;
            healthCounter.text = $"Health: {health} / 3";

            if (5 == currentBullet)
            {
                bulletSlider.value = 1;
            }
            else
            {
                currentRecharge += Time.deltaTime;
                if (currentRecharge > 1)
                {
                    currentRecharge = 0f;
                    currentBullet++;
                }
                bulletSlider.value = currentRecharge / 1;
            }
            bulletCounter.text = $"Bullets: {currentBullet} / 5";
        }

        void ShootBullet()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet >= 1)
            {
                currentBullet--;
                PrefabLoader.instance.CreateBullet(this, this.transform.position, Vector3.one, new(0, 7f));
            }
        }

        void FollowMouse()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            float minX = mainCamera.transform.position.x - cameraWidth / 2f;
            float maxX = mainCamera.transform.position.x + cameraWidth / 2f;
            float minY = mainCamera.transform.position.y - cameraHeight / 2f;
            float maxY = mainCamera.transform.position.y + cameraHeight / 2f;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            transform.position = targetPosition;
        }

        protected override void DeathEffect()
        {
            immune = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity target))
            {
                this.TakeDamage(null);
            }
        }
    }
}