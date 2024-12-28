using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyBox;

namespace Week4
{
    public enum Location { Home, Crossroads, Left, Right, You }

    public class Enemy : MonoBehaviour
    {
        public Location currentLocation { get; private set; }
        protected Location startLocation;
        int difficulty;

        private void Start()
        {
            difficulty = PlayerPrefs.GetInt(this.name);
            MoveToLocation(startLocation);
        }

        protected void MoveToLocation(Location location)
        {
            currentLocation = location;
            this.transform.SetParent(Player.instance.listOfLocations[(int)currentLocation]);
            this.transform.localPosition = SpawnPoint();
            StopAllCoroutines();

            float variable = 13.5f - difficulty;
            if (currentLocation == Location.You)
                Player.instance.GameOver("You Lost.", false);
            else if (difficulty > 0)
                StartCoroutine(WhileInRoom(Random.Range(variable, variable + 4f)));
        }

        protected virtual Vector2 SpawnPoint()
        {
            Vector2 result;
            do
            {
                result = new Vector2(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f));
            }
            while ((result.x > 0.2f && result.y < 0f) || (result.x < -0.2f && result.y > 0.2f));
            return result;
        }

        protected virtual IEnumerator WhileInRoom(float time)
        {
            yield return null;
        }
    }
}