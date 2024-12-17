using System.Collections.Generic;
using UnityEngine;

namespace Week3
{
    public class MovingPlat : MonoBehaviour
    {
        [SerializeField] Vector2 movingDirection;
        HashSet<Moving> toMove = new();

        private void FixedUpdate()
        {
            transform.Translate(movingDirection*Time.deltaTime);
            foreach (Moving moving in toMove)
                moving.cc.Move(movingDirection * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                movingDirection *= -1;
            }
            else if (other.TryGetComponent(out Moving target) && !toMove.Contains(target))
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
    }
}