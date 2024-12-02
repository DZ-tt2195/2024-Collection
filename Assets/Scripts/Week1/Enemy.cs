using UnityEngine;

namespace Week1
{
    public class Enemy : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(2, 0.5f, 0.75f, 4f);
        }
    }
}