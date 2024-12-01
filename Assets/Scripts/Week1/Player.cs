using UnityEngine;

namespace Week1
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera; // Assign the main camera in the Inspector
        [SerializeField] private float followSpeed; // Speed at which the player follows the mouse
        [SerializeField] private float zOffset = 0f; // Z position offset (optional, if 2D)

        void Update()
        {
            Vector3 mouseScreenPosition = Input.mousePosition;

            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                mainCamera.nearClipPlane + zOffset
            ));

            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            float minX = mainCamera.transform.position.x - cameraWidth / 2f;
            float maxX = mainCamera.transform.position.x + cameraWidth / 2f;
            float minY = mainCamera.transform.position.y - cameraHeight / 2f;
            float maxY = mainCamera.transform.position.y + cameraHeight / 2f;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}