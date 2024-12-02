using UnityEngine;
using System;
using MyBox;

namespace Week1
{
    public class Bullet : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        Entity owner;
        Action<Bullet> movement;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        internal void AssignInfo(Entity owner, Action<Bullet> movement)
        {
            this.owner = owner;
            this.movement = movement;
        }

        private void Update()
        {
            if (owner != null && movement != null)
                movement(this);

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
    }
}