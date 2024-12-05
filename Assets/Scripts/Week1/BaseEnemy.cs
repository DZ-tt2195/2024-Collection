using UnityEngine;

namespace Week1
{
    public class BaseEnemy : Entity
    {
        GameObject crossedOut;
        EnemyStat stat;

        public void EnemySetup(EnemyStat stat)
        {
            this.stat = stat;
            this.spriteRenderer.color = stat.bulletColor;
            this.Setup(stat.health, 0f, "Enemy");
            crossedOut = transform.GetChild(0).gameObject;
            crossedOut.SetActive(false);
            InvokeRepeating(nameof(ShootBullet), 1f, stat.attackRate);
        }

        protected virtual void ShootBullet()
        {
            Vector2 target = stat.customTarget ? stat.aim : AimAtPlayer();
            target.Normalize();
            WaveManager.instance.CreateBullet(this, stat.bulletColor,
                this.transform.position, stat.bulletSize, target * stat.bulletSpeed);
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