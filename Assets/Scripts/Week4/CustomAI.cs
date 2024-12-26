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

    private void Awake()
    {
        for (int i = 0; i < listOfSliders.Count; i++)
        {
            Slider slider = listOfSliders[i];
            slider.value = PlayerPrefs.GetInt(slider.transform.name);
        }
    }

    private void Update()
    {
        for (int i = 0; i < listOfSliders.Count; i++)
            sliderDisplay[i].text = $"{listOfSliders[i].value}";
    }

    public void LoadGame()
    {
        foreach (Slider slider in listOfSliders)
            PlayerPrefs.SetInt(slider.transform.parent.name, (int)slider.value);
        SceneManager.LoadScene("HorrorLevel");
    }
}
