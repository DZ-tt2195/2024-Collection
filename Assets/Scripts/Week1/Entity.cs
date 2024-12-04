using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using MyBox;

namespace Week1
{
    public class Entity : MonoBehaviour
    {
        public int health { get; private set; }
        protected SpriteRenderer spriteRenderer;
        protected bool immune = false;
        float immuneTime = 0f;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Setup(int number, float immuneTime, string tag)
        {
            this.health = number;
            this.tag = tag;
            this.immuneTime = immuneTime;
        }

        IEnumerator Immunity()
        {
            immune = true;
            float elapsedTime = 0f;
            bool flicker = true;

            while (elapsedTime < immuneTime)
            {
                flicker = !flicker;
                elapsedTime += Time.deltaTime;
                SetAlpha((flicker ? (elapsedTime / immuneTime) : 1f));
                yield return null;
            }
            SetAlpha(1);
            immune = false;
        }

        protected void SetAlpha(float alpha)
        {
            Color newColor = spriteRenderer.color;
            newColor.a = alpha;
            spriteRenderer.color = newColor;
        }

        public void TakeDamage(Bullet bullet)
        {
            if (immune || (bullet != null && this.CompareTag(bullet.tag)))
            {
                return;
            }

            health--;
            if (bullet != null)
                WaveManager.instance.ReturnBullet(bullet);

            if (health == 0)
                DeathEffect();
            else
                StartCoroutine(Immunity());
        }

        protected virtual void DeathEffect()
        {
        }
    }
}