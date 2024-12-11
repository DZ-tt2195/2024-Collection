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
    enum Slot { Blank, Star, Wall, Player, Death, Bounce, End }

    public class MapMaker : MonoBehaviour
    {
        [Foldout("Prefabs", true)]
        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject flagPrefab;
        [SerializeField] GameObject wallPrefab;
        [SerializeField] GameObject starPrefab;
        [SerializeField] GameObject deathPrefab;

        [Foldout("UI", true)]
        [SerializeField] Slider starSlider; 
        [SerializeField] TMP_Text starText;

        [Foldout("Brute force", true)]
        Slot[,] listOfSlots;
        [SerializeField] Transform storeTiles;
        HashSet<StoreStar> starLocations = new();
        List<List<Vector2Int>> solutions = new();
        List<Vector2Int> directions = new() { new(1, 0), new(-1, 0), new(0, -1), new(0, 1) };

        [Foldout("Gameplay", true)]
        bool canMove;
        GameObject player;
        HashSet<Vector2Int> playerStars = new();
        List<Vector2Int> playerPath = new();

        void Start()
        {
            CreateBoard();
            SimulateBoard();
            canMove = true;

            void CreateBoard()
            {
                BoardData data = Downloader.instance.puzzleLevels[0];
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
                                newDeath.transform.SetParent(storeTiles);
                                newDeath.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Death;
                                break;
                            case "Wall":
                                GameObject newWall = Instantiate(wallPrefab);
                                newWall.transform.SetParent(storeTiles);
                                newWall.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Wall;
                                break;
                            case "Star":
                                GameObject newStar = Instantiate(starPrefab);
                                newStar.transform.SetParent(storeTiles);
                                newStar.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Star;
                                starLocations.Add(new(newStar, new(i, j)));
                                break;
                            case "Player":
                                player = Instantiate(playerPrefab);
                                player.transform.SetParent(storeTiles);
                                player.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.Player;
                                break;
                            case "Flag":
                                GameObject newFlag = Instantiate(flagPrefab);
                                newFlag.transform.SetParent(storeTiles);
                                newFlag.transform.position = new(i, j);
                                listOfSlots[i, j] = Slot.End;
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
                    Debug.Log($"Shortest Path Found ({shortestList.Count-1}) : {solution}");
                }
                else
                {
                    Debug.Log("Puzzle impossible.");
                }
            }
        }

        private void Update()
        {
            if (canMove)
            {
                Vector2Int playerPosition = new((int)player.transform.position.x, (int)player.transform.position.y);

                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playerPath.Add(playerPosition);
                    StartCoroutine(Iteration(playerStars, playerPath, new(0, 1), false));
                }
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    playerPath.Add(playerPosition);
                    StartCoroutine(Iteration(playerStars, playerPath, new(0, -1), false));
                }
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    playerPath.Add(playerPosition);
                    StartCoroutine(Iteration(playerStars, playerPath, new(-1, 0), false));
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    playerPath.Add(playerPosition);
                    StartCoroutine(Iteration(playerStars, playerPath, new(1, 0), false));
                }
            }
        }

        IEnumerator Iteration(HashSet<Vector2Int> starsCollected, List<Vector2Int> currentPath, Vector2Int movement, bool simulated)
        {
            Vector2Int startPosition = currentPath[^1];
            Vector2Int currentPosition = startPosition;
            canMove = false;
            int gainedStars = 0;

            while (true)
            {
                bool AllStarsDone()
                {
                    foreach (StoreStar next in starLocations)
                    {
                        if (starsCollected.Contains(next.starPosition))
                        {
                            Debug.Log(next.starPosition);
                        }
                        if (!starsCollected.Contains(next.starPosition))
                        {
                            Debug.Log("didn't collect all stars");
                            return false;
                        }
                    }
                    Debug.Log("collected all stars");
                    return true;
                }

                if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.End && AllStarsDone())
                {
                    if (simulated)
                    {
                        List<Vector2Int> newList = new(currentPath) { currentPosition };
                        solutions.Add(newList);
                        yield break;
                    }
                    else
                    {
                        Debug.Log("victory");
                    }
                }
                else if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.Death)
                {
                    if (simulated)
                    {
                        yield break;
                    }
                    else
                    {
                        Debug.Log("you lost");
                        yield break;
                    }
                }
                else if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.Star)
                {
                    if (!starsCollected.Contains(currentPosition))
                    {
                        StoreStar target = starLocations.FirstOrDefault(next => next.starPosition.Equals(currentPosition));

                        starsCollected.Add(currentPosition);
                        gainedStars++;
                        if (!simulated)
                        {
                            target.starObject.SetActive(false);
                            starSlider.value = (float)starsCollected.Count / starLocations.Count;
                            starText.text = $"Stars: {starsCollected.Count}/{starLocations.Count}";
                        }
                    }
                }

                Vector2Int nextPosition = currentPosition + movement;
                if (listOfSlots[nextPosition.x, nextPosition.y] == Slot.Wall)
                    break;

                if (!simulated)
                    yield return MovePlayer(currentPosition, nextPosition);
                currentPosition = nextPosition;

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
            }

            List<Vector2Int> newPath = new();
            foreach (Vector2Int next in currentPath)
                newPath.Add(next);
            newPath.Add(currentPosition);

            HashSet<Vector2Int> newStars = new();
            foreach (Vector2Int next in starsCollected)
                newStars.Add(next);

            if (simulated && newPath.Count < 10)
            {
                if (currentPosition != startPosition || gainedStars > 0)
                {
                    foreach (Vector2Int next in directions)
                        StartCoroutine(Iteration(newStars, newPath, next, simulated));
                }
            }
            else if (!simulated)
            {
                playerStars = newStars;
                canMove = true;
            }
        }
    }
}