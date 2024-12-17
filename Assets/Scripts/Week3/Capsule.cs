using UnityEngine;
using TMPro;

namespace Week3
{
    public class Capsule : MonoBehaviour
    {
        SpriteRenderer platformDisplay;
        TMP_Text platformText;
        [SerializeField] SpriteRenderer platform;
        [SerializeField] int amount;

        private void Awake()
        {
            platformDisplay = transform.Find("Platform Display").GetComponent<SpriteRenderer>();
            platformDisplay.transform.localScale = platform.transform.localScale / 2f;
            platformDisplay.color = platform.color;
            platformText = transform.Find("Text").GetComponent<TMP_Text>();
            platformText.text = $"{amount}";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.NewPlatforms(platform, amount);
            }
        }
    }
}