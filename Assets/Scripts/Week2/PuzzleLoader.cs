using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Week2
{
    public class PuzzleLoader : MonoBehaviour
    {
        [SerializeField] List<Button> listOfButtons = new();

        private void Awake()
        {
            for (int i = 0; i < listOfButtons.Count; i++)
            {
                int number = i;
                listOfButtons[i].onClick.AddListener(() => LoadLevel(number));

                void LoadLevel(int n)
                {
                    PlayerPrefs.SetInt("Puzzle Level", n);
                    SceneManager.LoadScene("PuzzleLevel");
                }
            }
        }
    }
}
