using UnityEngine;
using System;

namespace Week1
{
    public class PrefabLoader : MonoBehaviour
    {
        public static PrefabLoader instance;
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] BaseEnemy enemyPrefab;

        private void Awake()
        {
            instance = this;
        }

        public void CreateEnemy(Vector2 start, EnemyStat stat)
        {
            BaseEnemy enemy = Instantiate(enemyPrefab);
            enemy.EnemySetup(stat);
            enemy.transform.position = start;
        }

        public void CreateBullet(Entity entity, Color color, Vector3 start, Vector3 scale, Vector3 direction)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.spriteRenderer.color = color;
            bullet.tag = entity.tag;
            bullet.transform.localScale = scale;
            bullet.transform.position = start;
            bullet.direction = direction;
        }
    }
}