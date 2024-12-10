using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MyBox;

[Serializable]
public class BoardData
{
    public string boardName;
    public string[,] terrain;

    public BoardData(string boardName, string[,] terrain)
    {
        this.boardName = boardName;
        this.terrain = terrain;
    }
}

public class Downloader : MonoBehaviour
{
    public static Downloader instance;
    string sheetURL = "1vfwvbFv3h_x6cH6DyEDnlNQ72RN7y7SOt4iJOe2PA3I";
    string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";
    [ReadOnly] public List<BoardData> puzzleLevels = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        DownloadEverything();
    }

    void DownloadEverything()
    {
        StartCoroutine(DownloadLevels(1));

        IEnumerator DownloadLevels(int number)
        {
            string folder = $"Week2/";
            string range = $"Level {number}";

            yield return Download(folder, range);
            try
            {
                puzzleLevels.Add(ReadMapData(range, ReadFile($"{folder}{range}")));
                StartCoroutine(DownloadLevels(number + 1));
            } catch { }
        }
    }

    string[][] ReadFile(string range)
    {
        TextAsset data = Resources.Load($"{range}") as TextAsset;

        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

        string[] numLines = editData.Split("[");
        string[][] list = new string[numLines.Length][];

        for (int i = 0; i < numLines.Length; i++)
            list[i] = numLines[i].Split("\",");
        
        return list;
    }

    BoardData ReadMapData(string name, string[][] data)
    {
        string[,] toReturn = new string[16, 9];

        for (int i = 0; i < data[1].Length; i++)
            data[1][i].Trim().Replace("\"", "");

        for (int i = 1; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                string nextObject = data[i][j].Replace("\"", "").Replace("\\", "").Replace("]", "").Trim();
                Debug.Log(nextObject);
                toReturn[j, i-1] = nextObject;
            }
        }

        return new BoardData(name, toReturn);
    }

    IEnumerator Download(string folder, string range)
    {
        if (Application.isEditor)
        {
            string url = $"{baseUrl}{sheetURL}/values/{range}?key={apiKey}";
            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Download failed {range}: {www.error}");
            }
            else
            {
                string filePath = $"Assets/Resources/{folder}{range}.txt";
                File.WriteAllText($"{filePath}", www.downloadHandler.text);

                string[] allLines = File.ReadAllLines($"{filePath}");
                List<string> modifiedLines = allLines.ToList();
                modifiedLines.RemoveRange(1, 3);
                File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
                Debug.Log($"downloaded {range}");
            }
        }
    }

}
