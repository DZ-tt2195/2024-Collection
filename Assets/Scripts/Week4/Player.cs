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

        [SerializeField] Slider powerSlider;
        [SerializeField] TMP_Text powerText;
        [SerializeField] Slider timeSlider;
        [SerializeField] TMP_Text timeText;

        float power = 1000f;
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

            power -= Time.deltaTime;
            if (leftDoor)
                power -= Time.deltaTime;
            if (rightDoor)
                power -= Time.deltaTime;
            if (camOn)
                power -= Time.deltaTime;
            powerSlider.value = power / 1000f;
            powerText.text = $"Power Left: {power:F0}";
        }
    }
}