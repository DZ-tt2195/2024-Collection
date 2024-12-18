using System.Collections.Generic;
using UnityEngine;

namespace Week3
{
    public class MovingPlat : MonoBehaviour
    {
        [SerializeField] Vector2 movingDirection;
        [SerializeField] float rotation;
        HashSet<Moving> toMove = new();

        private void FixedUpdate()
        {
            transform.Rotate(new Vector3(0, 0, rotation) * Time.deltaTime);
            transform.Translate(movingDirection*Time.deltaTime);
            foreach (Moving moving in toMove)
            {
                moving.applyForce.Add(action);
                Vector2 action()
                {
                    moving.applyForce.Remove(action);
                    return movingDirection;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 collisionNormal = (transform.position - other.ClosestPoint(transform.position)).normalized;

                if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y)) //wall
                    movingDirection = new Vector3(-movingDirection.x, movingDirection.y);
                else //floor or ceiling
                    movingDirection = new Vector2(movingDirection.x, -movingDirection.y);
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