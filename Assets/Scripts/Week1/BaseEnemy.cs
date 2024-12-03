using UnityEngine;

namespace Week1
{
    public class BaseEnemy : Entity
    {
        GameObject crossedOut;
        protected Vector2 bulletSize { get; private set; }
        protected float bulletSpeed { get; private set; }

        protected void EnemySetup(int health, Vector2 bulletSize, float attackRate, float bulletSpeed)
        {
            this.Setup(health, 0f, "Enemy");
            crossedOut = transform.GetChild(0).gameObject;
            crossedOut.SetActive(false);

            this.bulletSize = bulletSize;
            this.bulletSpeed = bulletSpeed;
            InvokeRepeating(nameof(ShootBullet), 1f, attackRate);
        }

        protected virtual void ShootBullet()
        {
            PrefabLoader.instance.CreateBullet(this, this.transform.position, bulletSize, AimAtPlayer() * bulletSpeed);
        }

        protected Vector2 AimAtPlayer()
        {
            return (Player.instance.transform.position - this.transform.position).normalized;
        }

        protected override void DeathEffect()
        {
            immune = true;
            crossedOut.SetActive(true);
            SetAlpha(0.5f);
        }
    }
}