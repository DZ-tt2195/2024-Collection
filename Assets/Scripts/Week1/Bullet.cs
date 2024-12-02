using UnityEngine;
using System;
using MyBox;

namespace Week1
{
    public class Bullet : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        public Vector3 direction;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            this.transform.Translate(direction * Time.deltaTime);
            float cameraHeight = 2f * Player.instance.mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * Player.instance.mainCamera.aspect;

            float minX = Player.instance.mainCamera.transform.position.x - cameraWidth / 2f;
            float maxX = Player.instance.mainCamera.transform.position.x + cameraWidth / 2f;
            float minY = Player.instance.mainCamera.transform.position.y - cameraHeight / 2f;
            float maxY = Player.instance.mainCamera.transform.position.y + cameraHeight / 2f;

            if (this.transform.position.x < minX || this.transform.position.x > maxX ||
                this.transform.position.y < minY || this.transform.position.y > maxY)
                Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Entity target))
            {
                if (!this.CompareTag(target.tag))
                {
                    target.TakeDamage();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}