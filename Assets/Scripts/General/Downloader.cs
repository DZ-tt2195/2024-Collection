using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using MyBox;
using TMPro;

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
            string folder = $"Week2";
            string range = $"W2 - Level {number}";

            yield return Download(folder, range);
            BoardData data = ReadMapData(folder, range);
            if (data != null)
            {
                puzzleLevels.Add(data);
                StartCoroutine(DownloadLevels(number + 1));
            }
        }
    }

    BoardData ReadMapData(string folder, string range)
    {
        string toLoad = $"{folder}/{range}";
        TextAsset data = Resources.Load(toLoad) as TextAsset;

        if (data == null)
            return null;

        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

        string[] numRows = editData.Split("[");
        string[][] list = new string[numRows.Length][];
        int maxCol = 0;

        for (int i = 0; i < numRows.Length; i++)
        {
            list[i] = numRows[i].Split("\",");
            if (list[i].Length > maxCol)
                maxCol = list[i].Length;
        }

        string[,] grid = new string[numRows.Length, maxCol];
        Debug.Log($"try {folder}/{range} | {grid.GetLength(0)}, {grid.GetLength(1)}");
        for (int x = 1; x < numRows.Length; x++)
        {
            for (int y = 0; y < maxCol; y++)
            {
                try
                {
                    grid[x - 1, y] = list[x][y].Trim().Replace("\"", "").Replace("]", "");
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }
        }
        Debug.Log($"return {folder}/{range} | {grid.GetLength(0)}, {grid.GetLength(1)}");
        return new BoardData(range, grid);
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
                string filePath = $"Assets/Resources/{folder}/{range}.txt";
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
