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
        public bool leftDoor { get; private set; }
        public bool rightDoor { get; private set; }
        public bool lightPath { get; private set; }
        public bool soundPath { get; private set; }
        public bool lightCenter { get; private set; }
        public bool soundCenter { get; private set; }

        [SerializeField] Slider timeSlider;
        [SerializeField] TMP_Text timeText;
        [SerializeField] Transform gameButtons;
        [SerializeField] Transform cameraMap;
        [SerializeField] Camera mainCam;

        float timePassed = 0f;
        float gameLength = 200f;
        bool gameOn = true;

        int currentCam = 0;
        bool camOn = false;

        public Enemy[] listOfEnemies { get; private set; }
        public List<Transform> listOfLocations = new();

        private void Awake()
        {
            instance = this;
            listOfEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            rightDoor = true;
            leftDoor = true;
        }

        private void Update()
        {
            if (!gameOn)
                return;

            timePassed += Time.deltaTime;
            timeSlider.value = (timePassed / gameLength);
            timeText.text = $"Time Left: {timePassed:F0}/{gameLength:F0}";

            if (timePassed > gameLength)
                GameOver("You Won!");

            cameraMap.gameObject.SetActive(camOn);
            gameButtons.gameObject.SetActive(!camOn);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                camOn = !camOn;
                Vector3 newPosition = camOn ? listOfLocations[currentCam].transform.position : listOfLocations[^1].transform.position;
                mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);
            }
        }

        public void GameOver(string text)
        {
            gameOn = false;
            Debug.Log(text);
            SwitchCamera(listOfLocations.Count-1);
            cameraMap.gameObject.SetActive(false);
            gameButtons.gameObject.SetActive(false);
            foreach (Enemy enemy in listOfEnemies)
                enemy.StopAllCoroutines();
        }

        public void SwitchCamera(int newCam)
        {
            currentCam = newCam;
            Vector3 newPosition = listOfLocations[currentCam].transform.position;
            mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);
        }

        public void ToggleLeftDoor(TMP_Text textBox)
        {
            leftDoor = !leftDoor;
            textBox.text = $"Left Door\n({(leftDoor ? "Closed" : "Open")})";
        }

        public void ToggleRightDoor(TMP_Text textBox)
        {
            rightDoor = !rightDoor;
            textBox.text = $"Right Door\n({(rightDoor ? "Closed" : "Open")})";
        }

        public void ToggleLightCenter()
        {
            lightCenter = true;
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.5f);
                lightCenter = false;
            }
        }

        public void ToggleSoundCenter()
        {
            soundCenter = true;
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.5f);
                soundCenter = false;
            }
        }

        public void ToggleLightPath()
        {
            lightPath = true;
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.5f);
                lightPath = false;
            }
        }

        public void ToggleSoundPath()
        {
            soundPath = true;
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.5f);
                soundPath = false;
            }
        }
    }
}