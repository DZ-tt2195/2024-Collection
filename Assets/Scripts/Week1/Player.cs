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

        int currentBullet = 5;
        [SerializeField] TMP_Text bulletCounter;

        float minX;
        float maxX;
        float minY;
        float maxY;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            this.Setup(5, 2f, "Player");

            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            minX = mainCamera.transform.position.x - cameraWidth / 2f;
            maxX = mainCamera.transform.position.x + cameraWidth / 2f;
            minY = mainCamera.transform.position.y - cameraHeight / 2f;
            maxY = 3.5f;
        }

        void Update()
        {
            if (health > 0)
            {
                FollowMouse();
                ShootBullet();
            }

            healthSlider.value = health / 5f;
            healthCounter.text = $"Health: {health} / 5";
            bulletCounter.text = $"Bullets: {currentBullet}";
        }

        void ShootBullet()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet >= 1)
            {
                currentBullet--;
                WaveManager.instance.CreateBullet(this, spriteRenderer.color, this.transform.position, Vector3.one, new(0, 7f));
            }
        }

        void FollowMouse()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            transform.position = targetPosition;
        }

        protected override void DeathEffect()
        {
            immune = true;
            SetAlpha(0.5f);
            WaveManager.instance.EndGame("You Lost.");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity entity))
            {
                this.TakeDamage(null);
            }
            else if (collision.TryGetComponent(out Resupply resupply))
            {
                WaveManager.instance.ReturnResupply(resupply);
                currentBullet += 2;
            }
        }
    }
}