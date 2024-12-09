using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

namespace Week2
{
    enum Slot { Blank, Star, Wall, Player, Death, Bounce, End }

    public class MapMaker : MonoBehaviour
    {
        Slot[,] listOfSlots;
        [SerializeField] HashSet<Vector2Int> starLocations = new();
        List<List<Vector2Int>> solutions = new();
        List<Vector2Int> directions = new() { new(1, 0), new(-1, 0), new(0, -1), new(0, 1) };

        [SerializeField] Vector2Int playerStart;
        [SerializeField] Vector2Int ending;
        [SerializeField] int rows = 19;
        [SerializeField] int cols = 11;

        void Start()
        {
            listOfSlots = new Slot[rows, cols];
            listOfSlots[ending.x, ending.y] = Slot.End;
            listOfSlots[ending.x, 0] = Slot.Death;

            foreach (Vector2Int next in directions)
                Iteration(new(), new() { playerStart }, next);

            List<Vector2Int> shortestList = solutions.OrderBy(list => list.Count).FirstOrDefault();
            if (shortestList != null)
            {
                string solution = "";
                for (int i = 1; i< shortestList.Count; i++)
                    solution += $"{shortestList[i]}, ";
                Debug.Log($"Shortest Path Found ({shortestList.Count-1}) : {solution}");
            }
            else
            {
                Debug.Log("Puzzle impossible.");
            }
        }

        void Iteration(List<Vector2Int> starsCollected, List<Vector2Int> currentPath, Vector2Int movement)
        {
            Vector2Int startPosition = currentPath[^1];
            Vector2Int currentPosition = startPosition;
            while (true)
            {
                if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.End && AllStarsDone())
                {
                    List<Vector2Int> newList = new(currentPath) { currentPosition };
                    solutions.Add(newList);
                    return;
                }
                if (listOfSlots[currentPosition.x, currentPosition.y] == Slot.Death)
                {
                    return;
                }
                bool AllStarsDone()
                {
                    /*
                    foreach (Vector2Int next in starsCollected)
                    {
                        if (!starLocations.Contains(next))
                            return false;
                    }*/
                    return true;
                }

                Vector2Int nextPosition = currentPosition + movement;
                if (nextPosition.x < 0 || nextPosition.x >= rows || nextPosition.y < 0 || nextPosition.y >= cols ||
                    listOfSlots[nextPosition.x, nextPosition.y] == Slot.Wall)
                    break;

                currentPosition = nextPosition;
            }

            List<Vector2Int> newPath = new(currentPath) { currentPosition };
            if (newPath.Count <= 10 && currentPosition != startPosition)
            {
                foreach (Vector2Int next in directions)
                    Iteration(starsCollected, newPath, next);
            }
        }
    }
}