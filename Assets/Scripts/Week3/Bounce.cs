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
    public class Bounce : MonoBehaviour
    {
        [SerializeField] Vector2 bounceEffect;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Moving target))
            {
                if (bounceEffect.x != 0)
                {
                    target.PushMe(bounceEffect, 0.33f);
                }
                if (bounceEffect.y != 0)
                {
                    target.ChangeYMovement(bounceEffect.y);
                }
            }
        }
    }
}