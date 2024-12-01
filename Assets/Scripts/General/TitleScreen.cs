using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"Scene {i}: {sceneName}");
        }
    }
}
