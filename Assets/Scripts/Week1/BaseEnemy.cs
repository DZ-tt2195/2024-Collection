using UnityEngine;

namespace Week1
{
    public class BaseEnemy : Entity
    {
        GameObject crossedOut;
        float bulletSize;
        float bulletSpeed;

        protected void EnemySetup(int health, float bulletSize, float attackRate, float bulletSpeed)
        {
            this.Setup(health, 0.1f, "Enemy");
            crossedOut = transform.GetChild(0).gameObject;
            crossedOut.SetActive(false);

            this.bulletSize = bulletSize;
            this.bulletSpeed = bulletSpeed;
            InvokeRepeating(nameof(ShootBullet), 1f, attackRate);
        }

        protected virtual void ShootBullet()
        {
            Vector2 direction = (Player.instance.transform.position - this.transform.position).normalized;
            PrefabLoader.instance.CreateBullet(this, this.transform.position, bulletSize, direction * bulletSpeed);
        }

        protected override void DeathEffect()
        {
            immune = true;
            crossedOut.SetActive(true);
            SetAlpha(0.5f);
        }
    }
}