using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using MyBox;

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

        [Foldout("Sounds", true)]
        [SerializeField] AudioClip doorOpen;
        [SerializeField] AudioClip doorClose;
        [SerializeField] AudioClip cameraSound;
        [SerializeField] AudioClip lightPathSound;
        [SerializeField] AudioClip soundPathSound;
        [SerializeField] AudioClip lightCenterSound;
        [SerializeField] AudioClip soundCenterSound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;

        [Foldout("UI", true)]
        [SerializeField] Slider timeSlider;
        [SerializeField] TMP_Text timeText;
        [SerializeField] Slider cameraSlider;
        [SerializeField] TMP_Text cameraText;
        [SerializeField] Transform gameButtons;
        [SerializeField] Transform cameraMap;
        [SerializeField] Camera mainCam;
        [SerializeField] TMP_Text endText;

        [Foldout("Misc", true)]
        float timePassed = 0f;
        float gameLength = 200f;
        float cameraPower;
        float maxPower = 50f;
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
            timeSlider.value = timePassed / gameLength;
            timeText.text = $"Time Left: {timePassed:F1}";

            if (timePassed > gameLength)
            {
                GameOver("You Won!", true);
                return;
            }

            if (cameraPower == 0f)
                camOn = false;
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                camOn = !camOn;
                if (camOn)
                    AudioManager.instance.PlaySound(cameraSound, 0.35f);
            }

            Vector3 newPosition = camOn ? listOfLocations[currentCam].transform.position : listOfLocations[^1].transform.position;
            mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);

            cameraMap.gameObject.SetActive(camOn);
            gameButtons.gameObject.SetActive(!camOn);

            if (camOn)
                cameraPower -= Time.deltaTime;
            cameraPower = Mathf.Max(0f, cameraPower);
            cameraSlider.value = cameraPower / maxPower;
            cameraText.text = $"Camera Battery: {cameraPower:F1}";
        }

        public void GameOver(string text, bool won)
        {
            gameOn = false;
            endText.transform.parent.gameObject.SetActive(true);
            endText.text = text;

            cameraMap.gameObject.SetActive(false);
            gameButtons.gameObject.SetActive(false);
            AudioManager.instance.PlaySound(won ? winSound : loseSound, 0.35f);

            Vector3 newPosition = listOfLocations[^1].transform.position;
            mainCam.transform.localPosition = new(newPosition.x, newPosition.y, -10);
            foreach (Enemy enemy in listOfEnemies)
                enemy.StopAllCoroutines();
        }

        public void SwitchCamera(int newCam)
        {
            currentCam = newCam;
            AudioManager.instance.PlaySound(cameraSound, 0.35f);
        }

        public void ToggleLeftDoor(TMP_Text textBox)
        {
            leftDoor = !leftDoor;
            textBox.text = $"Left Door\n({(leftDoor ? "Closed" : "Open")})";
            DoorSound(leftDoor);
        }

        public void ToggleRightDoor(TMP_Text textBox)
        {
            rightDoor = !rightDoor;
            textBox.text = $"Right Door\n({(rightDoor ? "Closed" : "Open")})";
            DoorSound(rightDoor);
        }

        void DoorSound(bool open)
        {
            AudioManager.instance.PlaySound(open ? doorOpen : doorClose, 0.35f);
        }

        public void ToggleLightCenter()
        {
            lightCenter = true;
            AudioManager.instance.PlaySound(lightCenterSound, 0.35f);
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.35f);
                lightCenter = false;
            }
        }

        public void ToggleSoundCenter()
        {
            soundCenter = true;
            AudioManager.instance.PlaySound(soundCenterSound, 0.35f);
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.35f);
                soundCenter = false;
            }
        }

        public void ToggleLightPath()
        {
            lightPath = true;
            AudioManager.instance.PlaySound(lightPathSound, 0.35f);
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.35f);
                lightPath = false;
            }
        }

        public void ToggleSoundPath()
        {
            soundPath = true;
            AudioManager.instance.PlaySound(soundPathSound, 0.35f);
            StartCoroutine(Disable());

            IEnumerator Disable()
            {
                yield return new WaitForSeconds(0.35f);
                soundPath = false;
            }
        }
    }
}