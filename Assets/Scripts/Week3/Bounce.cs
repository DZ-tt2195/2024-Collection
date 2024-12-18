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
                    target.ModifyGroundSpeed(bounceEffect.x);
                    target.StartCoroutine(WearOff(target));

                    IEnumerator WearOff(Moving target)
                    {
                        yield return new WaitForSeconds(0.5f);
                        target.ModifyGroundSpeed(-bounceEffect.x);
                    }
                }
                if (bounceEffect.y != 0)
                {
                    target.ChangeYMovement(bounceEffect.y);
                }
            }
        }
    }
}