using UnityEngine;

namespace Week1
{
    public class Enemy : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(3, new(0.5f, 0.5f), 1, 3.5f);
        }
    }
}