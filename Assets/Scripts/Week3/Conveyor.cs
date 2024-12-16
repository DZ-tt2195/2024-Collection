using UnityEngine;

namespace Week3
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] float moveEffect;
        float currentModify = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && currentModify == 0f)
            {
                currentModify = moveEffect;
                player.ModifyGroundSpeed(currentModify);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && currentModify != 0f)
            {
                player.ModifyGroundSpeed(-currentModify);
                currentModify = 0f;
            }
        }
    }
}