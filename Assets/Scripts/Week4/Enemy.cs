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
        [PositiveValueOnly] [MaxValue(10)] [SerializeField] int difficulty;

        private void Start()
        {
            MoveToLocation(Location.Home);
        }

        protected void MoveToLocation(Location location)
        {
            currentLocation = location;
            this.transform.SetParent(Player.instance.listOfLocations[(int)currentLocation]);
            this.transform.localPosition = SpawnPoint();
            StopAllCoroutines();

            float waitTime = Random.Range(15f, 30f) - Random.Range(Mathf.Ceil(difficulty / 2f), difficulty);
            Debug.Log($"{this.name}: {currentLocation} ({waitTime})");
            if (difficulty > 0)
                StartCoroutine(WhileInRoom(waitTime));
        }

        protected virtual Vector2 SpawnPoint()
        {
            return new(Random.Range(-0.35f, 0.35f), Random.Range(-0.35f, 0.35f));
        }

        protected virtual IEnumerator WhileInRoom(float time)
        {
            yield return null;
        }
    }
}