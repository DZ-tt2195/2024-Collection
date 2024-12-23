using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week4
{
    public enum Location { Home, Crossroads, Center, Left, Right, You }

    public class Enemy : MonoBehaviour
    {
        public Location currentLocation { get; private set; }
        int difficulty;

        private void Awake()
        {
            MoveToLocation(Location.Home);
        }

        protected void MoveToLocation(Location location)
        {
            currentLocation = location;
            StopAllCoroutines();

            float waitTime = 20 - Random.Range(difficulty / 4f, difficulty);
            if (difficulty > 0)
                StartCoroutine(WhileInRoom(waitTime));
        }

        protected virtual IEnumerator WhileInRoom(float time)
        {
            yield return null;
        }
    }
}