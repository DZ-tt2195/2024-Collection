using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] List<Button> listOfButtons = new();
    [Scene] [SerializeField] string sceneName;

    private void Awake()
    {
        for (int i = 0; i < listOfButtons.Count; i++)
        {
            int number = i;
            listOfButtons[i].onClick.AddListener(() => LoadLevel(number));

            void LoadLevel(int n)
            {
                PlayerPrefs.SetInt("Level", n);
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
