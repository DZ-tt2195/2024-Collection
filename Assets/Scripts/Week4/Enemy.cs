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

            float waitTime = Random.Range(5f, 15f) - Random.Range(0f, difficulty);
            //Debug.Log($"{this.name}: {currentLocation} ({waitTime})");
            if (currentLocation == Location.You)
                Player.instance.GameOver("You Lost.");
            else if (difficulty > 0)
                StartCoroutine(WhileInRoom(Mathf.Clamp(waitTime, 5f, 15f)));
        }

        protected virtual Vector2 SpawnPoint()
        {
            return new(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
        }

        protected virtual IEnumerator WhileInRoom(float time)
        {
            yield return null;
        }
    }
}