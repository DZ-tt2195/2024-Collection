using UnityEngine;

namespace Week1
{
    public class Enemy : Entity
    {
        protected override void Awake()
        {
            base.Awake();
            this.SetHealth(2, 0.5f, "Enemy");
            InvokeRepeating(nameof(ShootBullet), 1f, 0.75f);
        }

        void ShootBullet()
        {
            Vector2 direction = (Player.instance.transform.position - this.transform.position);
            direction.Normalize();
            PrefabLoader.instance.CreateBullet(this, this.transform.position, 0.5f, direction*3.5f);
        }
    }
}