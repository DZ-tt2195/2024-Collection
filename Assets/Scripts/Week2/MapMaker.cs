using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections;

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
        Slot[,] listOfSlots;
        GameObject player;

        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject flagPrefab;
        [SerializeField] GameObject wallPrefab;
        [SerializeField] GameObject starPrefab;

        bool canMove;

        HashSet<StoreStar> starLocations = new();
        List<List<Vector2Int>> solutions = new();
        List<Vector2Int> directions = new() { new(1, 0), new(-1, 0), new(0, -1), new(0, 1) };

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
                        switch (data.terrain[i, j])
                        {
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
                                listOfSlots[i, j] = Slot.End;
                                break;
                            default:
                                break;
                        }
                    }
                }
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
                    Debug.Log($"Shortest Path Found ({shortestList.Count - 1}) : {solution}");
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
                        if (!starsCollected.Contains(next.starPosition))
                        {
                            Debug.Log("didn't collect all stars");
                            return false;
                        }
                    }
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
                }
                else if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.Star)
                {
                    if (!starsCollected.Contains(currentPosition))
                    {
                        StoreStar target = starLocations.FirstOrDefault(next => next.starPosition.Equals(currentPosition));

                        if (!starsCollected.Contains(currentPosition))
                        {
                            starsCollected.Add(currentPosition);
                            if (simulated)
                                gainedStars++;
                            else
                                target.starObject.SetActive(false);
                        }
                    }
                }

                Vector2Int nextPosition = currentPosition + movement;
                if (nextPosition.x <= 0 || nextPosition.x >= listOfSlots.GetLength(0)-1 || nextPosition.y <= 0 || nextPosition.y >= listOfSlots.GetLength(1)-1 ||
                    listOfSlots[nextPosition.x, nextPosition.y] == Slot.Wall)
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

            List<Vector2Int> newPath = new(currentPath) { currentPosition };
            if (simulated && newPath.Count < 10 && (currentPosition != startPosition || gainedStars > 0))
            {
                foreach (Vector2Int next in directions)
                    StartCoroutine(Iteration(starsCollected, newPath, next, simulated));
            }
            else if (!simulated)
            {
                playerStars = starsCollected;
                canMove = true;
            }
        }
    }
}