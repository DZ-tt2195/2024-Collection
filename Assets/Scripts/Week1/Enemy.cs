using UnityEngine;

namespace Week1
{
    public class Enemy : Entity
    {
        private void Awake()
        {
            this.SetHealth(2);
            InvokeRepeating(nameof(ShootBullet), 1f, 0.5f);
        }

        void ShootBullet()
        {
            Vector2 direction = (Player.instance.transform.position - this.transform.position);
            direction.Normalize();
            PrefabLoader.instance.CreateBullet(this, this.transform.position, 1, direction*5);
        }
    }
}