using UnityEngine;

namespace Week1
{
    public class Entity : MonoBehaviour
    {
        public int health { get; private set; }

        public virtual void SetHealth(int number)
        {
            this.health = number;
        }

        public void TakeDamage()
        {
            health--;
            if (health == 0)
                DeathEffect();
        }

        protected virtual void DeathEffect()
        {
            Destroy(this.gameObject);
        }
    }
}