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
        public CharacterController cc { get; private set; }
        [SerializeField] protected float moveSpeed;
        [SerializeField] float gravity;
        public float groundModify { get; private set; }
        public float yMovement { get; private set; }

        protected virtual void Awake()
        {
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

        protected virtual Vector2 MoveMe()
        {
            Vector2 movement = new(groundModify + moveSpeed, yMovement);
            return movement;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y < -0.9f && yMovement > 0)
            {
                ChangeYMovement(0);
            }
        }

        public void ModifyGroundSpeed(float speed)
        {
            groundModify += speed;
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