using UnityEngine;

namespace Week1
{
    public class Enemy3 : BaseEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            this.EnemySetup(4, 1.25f, 1.25f, 1.5f);
        }
    }
}
