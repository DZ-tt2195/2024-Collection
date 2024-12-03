using UnityEngine;

namespace Week1
{
    public class Enemy2 : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(2, new(0.5f,0.5f), 0.5f, 2f);
        }
    }
}
