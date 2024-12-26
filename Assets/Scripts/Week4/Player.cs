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
        [SerializeField] Transform gameButtons;
        [SerializeField] Transform cameraMap;
        [SerializeField] Camera mainCam;

        float timePassed = 0f;
        int currentCam = 0;
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
            timePassed += Time.deltaTime;
            timeSlider.value = (timePassed / 300f);
            timeText.text = $"Time Left: {timePassed:F0}/300";

            cameraMap.gameObject.SetActive(camOn);
            gameButtons.gameObject.SetActive(!camOn);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                camOn = !camOn;
                Vector3 newPosition = camOn ? listOfLocations[currentCam].transform.position : listOfLocations[^1].transform.position;
                mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);
            }
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