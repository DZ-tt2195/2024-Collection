using UnityEngine;
using System;

namespace Week1
{
    public class PrefabLoader : MonoBehaviour
    {
        public static PrefabLoader instance;
        [SerializeField] Bullet bulletPrefab;

        private void Awake()
        {
            instance = this;
        }

        public void CreateBullet(Entity entity, Vector3 start, Vector3 scale, Vector3 direction)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.tag = entity.tag;
            bullet.transform.localScale = scale;
            bullet.transform.position = start;
            bullet.direction = direction;
        }
    }
}