using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class CustomAI : MonoBehaviour
{
    [SerializeField] List<Slider> listOfSliders = new();
    [SerializeField] List<TMP_Text> sliderDisplay = new();

    private void Update()
    {
        for (int i = 0; i < listOfSliders.Count; i++)
            sliderDisplay[i].text = $"{listOfSliders[i].value}";
    }

    public void LoadGame()
    {
        foreach (Slider slider in listOfSliders)
            PlayerPrefs.SetInt(slider.name, (int)slider.value);
        SceneManager.LoadScene("HorrorLevel");
    }
}
