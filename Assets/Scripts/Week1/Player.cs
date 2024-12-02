using UnityEngine;

namespace Week1
{
    public class Player : Entity
    {
        public static Player instance;
        public Camera mainCamera; // Assign the main camera in the Inspector

        private void Awake()
        {
            instance = this;
            this.SetHealth(3);
        }

        void Update()
        {
            FollowMouse();
            ShootBullet();
        }

        void ShootBullet()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                for (int i = -2; i<=2; i++)
                    PrefabLoader.instance.CreateBullet(this, this.transform.position, 1, new(i, 7.5f));
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
        }
    }
}