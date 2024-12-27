using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
        [SerializeField] Slider cameraSlider;
        [SerializeField] TMP_Text cameraText;
        [SerializeField] Transform gameButtons;
        [SerializeField] Transform cameraMap;
        [SerializeField] Camera mainCam;

        float timePassed = 0f;
        float gameLength = 180f;
        float cameraPower;
        float maxPower = 50f;
        bool gameOn = true;

        int currentCam = 0;
        bool camOn = false;
        [SerializeField] TMP_Text endText;

        public Enemy[] listOfEnemies { get; private set; }
        public List<Transform> listOfLocations = new();

        private void Awake()
        {
            instance = this;
            listOfEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            rightDoor = true;
            leftDoor = true;
            cameraPower = maxPower;
        }

        public void OnButtonClick()
        {
            EventSystem.current.SetSelectedGameObject(null);
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

            if (cameraPower == 0f)
                camOn = false;
            else if (Input.GetKeyDown(KeyCode.Space))
                camOn = !camOn;

            Vector3 newPosition = camOn ? listOfLocations[currentCam].transform.position : listOfLocations[^1].transform.position;
            mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);

            cameraMap.gameObject.SetActive(camOn);
            gameButtons.gameObject.SetActive(!camOn);

            if (camOn)
                cameraPower -= Time.deltaTime;
            cameraPower = Mathf.Max(0f, cameraPower);
            cameraSlider.value = cameraPower / maxPower;
            cameraText.text = $"Camera Battery: {cameraPower:F0}";
        }

        public void GameOver(string text)
        {
            gameOn = false;
            endText.transform.parent.gameObject.SetActive(true);
            endText.text = text;

            cameraMap.gameObject.SetActive(false);
            gameButtons.gameObject.SetActive(false);

            Vector3 newPosition = listOfLocations[^1].transform.position;
            mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);
            foreach (Enemy enemy in listOfEnemies)
                enemy.StopAllCoroutines();
        }

        public void SwitchCamera(int newCam)
        {
            currentCam = newCam;
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