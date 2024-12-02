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

        public void CreateBullet(Entity entity, Action<Bullet> action)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.transform.position = entity.transform.position;
            bullet.AssignInfo(entity, action);
        }
    }
}