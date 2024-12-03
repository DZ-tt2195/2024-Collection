using UnityEngine;

namespace Week1
{
    public class Enemy4 : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(3, new(1, 3.5f), 1.5f, 4f);
        }

        protected override void ShootBullet()
        {
            PrefabLoader.instance.CreateBullet(this, transform.position, bulletSize, new Vector2(0,-1) * bulletSpeed);
        }
    }
}