using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections;
using MyBox;
using TMPro;
using UnityEngine.UI;

public class StoreStar
{
    public GameObject starObject;
    public Vector2Int starPosition;

    public StoreStar(GameObject starObject, Vector2Int starPosition)
    {
        this.starObject = starObject;
        this.starPosition = starPosition;
    }
}

namespace Week2
{
    enum Slot { Blank, Star, Wall, Player, Death, Flag, Left, Right, Up, Down, Stop }

    public class MapMaker : MonoBehaviour
    {
        /*
        void OnEnable()
        {
            Application.logMessageReceived += DebugMessages;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= DebugMessages;
        }

        void DebugMessages(string logString, string stackTrace, LogType type)
        {
                debugText.text += ($"{logString} | {stackTrace}\n");
        }
        */

#region Variables

        [SerializeField] TMP_Text debugText;

        [Foldout("Prefabs", true)]
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject flagPrefab;
        [SerializeField] GameObject wallPrefab;
        [SerializeField] GameObject starPrefab;
        [SerializeField] GameObject deathPrefab;
        [SerializeField] GameObject arrowPrefab;
        [SerializeField] GameObject stopPrefab;

        [Foldout("UI", true)]
        [SerializeField] Slider starSlider; 
        [SerializeField] TMP_Text starText;
        [SerializeField] TMP_Text endText;
        [SerializeField] TMP_Text moveCounter;
        [SerializeField] TMP_Text minimumMoves;

        [Foldout("Brute force", true)]
        Slot[,] listOfSlots;
        HashSet<StoreStar> starLocations = new();
        List<List<Vector2Int>> solutions = new();
        List<Vector2Int> directions = new() { new(1, 0), new(-1, 0), new(0, -1), new(0, 1) };

        [Foldout("Gameplay", true)]
        bool canMove;
        GameObject player;
        HashSet<Vector2Int> playerStars = new();
        List<Vector2Int> playerPath = new();

        #endregion

#region Setup

        void Start()
        {
            moveCounter.text = $"Moves: 0";

            CreateBoard();
            SimulateBoard();
            canMove = true;

            void CreateBoard()
            {
                BoardData data = Downloader.instance.puzzleLevels[PlayerPrefs.GetInt("Level")];
                listOfSlots = new Slot[data.terrain.GetLength(0), data.terrain.GetLength(1)];

                for (int i = 0; i < data.terrain.GetLength(0); i++)
                {
                    for (int j = 0; j < data.terrain.GetLength(1); j++)
                    {
                        if (data.terrain[i, j] == null)
                            continue;

                        switch (data.terrain[i, j].Trim())
                        {
                            case "Death":
                                GameObject newDeath = Instantiate(deathPrefab);
                                newDeath.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Death;
                                break;
                            case "Stop":
                                GameObject newStop = Instantiate(stopPrefab);
                                newStop.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Stop;
                                break;
                            case "Wall":
                                GameObject newWall = Instantiate(wallPrefab);
                                newWall.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Wall;
                                break;
                            case "Star":
                                GameObject newStar = Instantiate(starPrefab);
                                newStar.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Star;
                                starLocations.Add(new(newStar, new(i, j)));
                                break;
                            case "Player":
                                player = Instantiate(playerPrefab);
                                player.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Player;
                                break;
                            case "Flag":
                                GameObject newFlag = Instantiate(flagPrefab);
                                newFlag.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Flag;
                                break;
                            case "Left":
                                GameObject leftArrow = Instantiate(arrowPrefab);
                                leftArrow.transform.position = new(i, j);
                                leftArrow.transform.localEulerAngles = new(0, 0, 180);
                                listOfSlots[i, j] = Slot.Left;
                                break;
                            case "Right":
                                GameObject rightArrow = Instantiate(arrowPrefab);
                                rightArrow.transform.position = new(i, j);
                                rightArrow.transform.localEulerAngles = new(0, 0, 0);
                                listOfSlots[i, j] = Slot.Right;
                                break;
                            case "Down":
                                GameObject downArrow = Instantiate(arrowPrefab);
                                downArrow.transform.position = new(i, j);
                                downArrow.transform.localEulerAngles = new(0, 0, 270);
                                listOfSlots[i, j] = Slot.Down;
                                break;
                            case "Up":
                                GameObject upArrow = Instantiate(arrowPrefab);
                                upArrow.transform.position = new(i, j);
                                upArrow.transform.localEulerAngles = new(0, 0, 90);
                                listOfSlots[i, j] = Slot.Up;
                                break;
                            default:
                                break;
                        }
                    }
                }

                starSlider.value = 0;
                starText.text = $"Stars: 0/{starLocations.Count}";
            }

            void SimulateBoard()
            {
                foreach (Vector2Int next in directions)
                    StartCoroutine(Iteration(new(), new() { new((int)player.transform.position.x, (int)player.transform.position.y) }, next, true));

                List<Vector2Int> shortestList = solutions.OrderBy(list => list.Count).FirstOrDefault();
                if (shortestList != null)
                {
                    string solution = "";
                    for (int i = 1; i < shortestList.Count; i++)
                        solution += $"{shortestList[i]}, ";
                    Debug.Log(solution);
                    minimumMoves.text = $"Minimum Moves: {shortestList.Count - 1}";
                }
                else
                {
                    Debug.Log("Puzzle impossible.");
                    minimumMoves.text = $"Minimum Moves: X";
                }
            }
        }

        #endregion

#region Gameplay

        private void Update()
        {
            if (canMove)
            {
                Vector2Int playerPosition = new((int)player.transform.position.x, (int)player.transform.position.y);

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playerPath.Add(playerPosition);
                    moveCounter.text = $"Moves: {playerPath.Count}";
                    StartCoroutine(Iteration(playerStars, playerPath, new(0, 1), false));
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    playerPath.Add(playerPosition);
                    moveCounter.text = $"Moves: {playerPath.Count}";
                    StartCoroutine(Iteration(playerStars, playerPath, new(0, -1), false));
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    playerPath.Add(playerPosition);
                    moveCounter.text = $"Moves: {playerPath.Count}";
                    StartCoroutine(Iteration(playerStars, playerPath, new(-1, 0), false));
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    playerPath.Add(playerPosition);
                    moveCounter.text = $"Moves: {playerPath.Count}";
                    StartCoroutine(Iteration(playerStars, playerPath, new(1, 0), false));
                }
            }
        }

        IEnumerator Iteration(HashSet<Vector2Int> starsCollected, List<Vector2Int> currentPath, Vector2Int movement, bool simulated)
        {
            Vector2Int startPosition = currentPath[^1];
            Vector2Int currentPosition = startPosition;
            Vector2Int currentDirection = movement;

            canMove = false;
            List<Vector2Int> routeStar = new();

            while (true)
            {
                switch (listOfSlots[currentPosition.x, currentPosition.y])
                {
                    case Slot.Flag:
                        if (simulated)
                        {
                            if (starsCollected.Count == starLocations.Count)
                            {
                                List<Vector2Int> newList = new(currentPath) { currentPosition };
                                solutions.Add(newList);
                                yield break;
                            }
                        }
                        else
                        {
                            if (starsCollected.Count + routeStar.Count == starLocations.Count)
                            {
                                endText.transform.parent.gameObject.SetActive(true);
                                endText.text = "You Win!";
                                yield break;
                            }
                        }
                        break;
                    case Slot.Death:
                        if (!simulated)
                        {
                            endText.transform.parent.gameObject.SetActive(true);
                            endText.text = "You Lost.";
                        }
                        yield break;
                    case Slot.Star:
                        if (!starsCollected.Contains(currentPosition) && !routeStar.Contains(currentPosition))
                        {
                            StoreStar target = starLocations.FirstOrDefault(next => next.starPosition.Equals(currentPosition));
                            routeStar.Add(new(target.starPosition.x, target.starPosition.y));

                            if (!simulated)
                            {
                                starSlider.value = (float)(starsCollected.Count + routeStar.Count) / starLocations.Count;
                                starText.text = $"Stars: {starsCollected.Count + routeStar.Count}/{starLocations.Count}";
                                target.starObject.SetActive(false);
                            }
                        }
                        break;
                    case Slot.Up:
                        currentDirection = new(0, 1);
                        break;
                    case Slot.Down:
                        currentDirection = new(0, -1);
                        break;
                    case Slot.Right:
                        currentDirection = new(1, 0);
                        break;
                    case Slot.Left:
                        currentDirection = new(-1, 0);
                        break;
                    default:
                        break;
                }

                Vector2Int nextPosition = currentPosition + currentDirection;
                if (listOfSlots[nextPosition.x, nextPosition.y] == Slot.Wall)
                    break;

                if (!simulated)
                {
                    yield return MovePlayer(currentPosition, nextPosition);
                }
                IEnumerator MovePlayer(Vector2 playerStart, Vector2 playerEnd)
                {
                    float elapsedTime = 0f;
                    player.transform.position = playerStart;
                    while (elapsedTime < 0.05f)
                    {
                        player.transform.position = Vector2.Lerp(playerStart, playerEnd, elapsedTime / 0.1f);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                    player.transform.position = playerEnd;
                }

                currentPosition = nextPosition;

                if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.Stop)
                    break;
            }

            List<Vector2Int> newPath = new(currentPath) { currentPosition };
            HashSet<Vector2Int> newStars = new(starsCollected.Union(routeStar));

            if (simulated && newPath.Count <= 10)
            {
                if (currentPosition != startPosition || routeStar.Count > 0)
                {
                    foreach (Vector2Int next in directions)
                        StartCoroutine(Iteration(newStars, newPath, next, simulated));
                }
            }
            else if (!simulated)
            {
                starSlider.value = (float)newStars.Count / starLocations.Count;
                starText.text = $"Stars: {newStars.Count}/{starLocations.Count}";

                playerStars = newStars;
                canMove = true;
            }
        }
    }

    #endregion

}