using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneCheck : MonoBehaviour
{
    Button button;
    [SerializeField] string toLoad;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (toLoad.Equals(sceneName))
            {
                button.interactable = true;
                button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
                break;
            }
        }
    }
}
