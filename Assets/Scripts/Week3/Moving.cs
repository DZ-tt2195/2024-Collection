using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.InputSystem;

namespace Week3
{
    public class Moving : MonoBehaviour
    {
        [Foldout("Physics", true)]
        protected CharacterController cc { get; private set; }
        [SerializeField] protected float moveSpeed;
        [SerializeField] float gravity;
        protected bool disableMainMove { get; private set; }
        protected float yMovement { get; private set; }
        public List<Func<Vector2>> applyForce { get; private set; }

        protected virtual void Awake()
        {
            applyForce = new();
            cc = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            if (!cc.isGrounded)
                yMovement -= gravity;

            if (yMovement < -30)
                yMovement = -30;

            cc.Move(MoveMe() * Time.deltaTime);
        }

        Vector2 MoveMe()
        {
            Vector2 total = Vector2.zero;
            for (int i = applyForce.Count - 1; i >= 0; i--)
            {
                Vector2 application = applyForce[i]();
                total.x += application.x;
                total.y += application.y;
            }

            return total;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y < -0.9f && yMovement > 0)
            {
                ChangeYMovement(0);
            }
        }

        public void PushMe(Vector2 push, float time)
        {
            Vector2 action() => (push);
            applyForce.Add(action);
            disableMainMove = true;
            this.StartCoroutine(RemoveMethod());

            IEnumerator RemoveMethod()
            {
                yield return new WaitForSeconds(time);
                disableMainMove = false;
                applyForce.Remove(action);
            }
        }

        public void ChangeYMovement(float amount)
        {
            yMovement = amount;
        }

        protected void ChangeAlpha(SpriteRenderer sr, float alpha)
        {
            Color newColor = sr.color;
            newColor.a = alpha;
            sr.color = newColor;
        }
    }
}