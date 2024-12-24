using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace Week4
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        public bool leftDoor;
        public bool rightDoor;
        public bool lightPath;
        public bool soundPath;
        public bool lightCenter;
        public bool soundCenter;

        [SerializeField] Slider timeSlider;
        [SerializeField] TMP_Text timeText;

        float time = 0f;
        bool camOn = false;

        public Enemy[] listOfEnemies;
        public List<Transform> listOfLocations = new();

        private void Awake()
        {
            instance = this;
            listOfEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        }

        private void Update()
        {
            time += Time.deltaTime;
            timeSlider.value = (time / 360f);
            timeText.text = $"Time Left: {time:F0} sec";
        }
    }
}