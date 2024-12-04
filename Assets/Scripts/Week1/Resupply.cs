using UnityEngine;

namespace Week1
{
    public class Resupply : MonoBehaviour
    {
        private void Update()
        {
            this.transform.Translate(new Vector2(0, -1) * Time.deltaTime);
            float cameraHeight = 2f * Player.instance.mainCamera.orthographicSize;
            float minY = Player.instance.mainCamera.transform.position.y - cameraHeight / 2f;
            if (this.transform.position.y < minY)
                WaveManager.instance.ReturnResupply(this);
        }
    }
}