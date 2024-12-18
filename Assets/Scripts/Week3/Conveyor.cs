using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Week3
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] Vector2 moveEffect;
        HashSet<Moving> toMove = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Moving target) && !toMove.Contains(target))
            {
                toMove.Add(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Moving target) && toMove.Contains(target))
            {
                toMove.Remove(target);
            }
        }

        private void FixedUpdate()
        {
            foreach (Moving moving in toMove)
            {
                moving.applyForce.Add(action);
                Vector2 action()
                {
                    moving.applyForce.Remove(action);
                    return moveEffect;
                }
            }
        }
    }
}