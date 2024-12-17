using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Week3
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] float moveEffect;
        HashSet<Moving> toMove = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Moving target) && !toMove.Contains(target))
            {
                toMove.Add(target);
                target.ModifyGroundSpeed(moveEffect);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Moving target) && toMove.Contains(target))
            {
                toMove.Remove(target);
                target.ModifyGroundSpeed(-moveEffect);
            }
        }
    }
}