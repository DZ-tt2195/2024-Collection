using UnityEngine;

namespace Week1
{
    public class Enemy3 : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(4, new(1.5f,1.5f), 1.5f, 2.5f);
        }
    }
}
