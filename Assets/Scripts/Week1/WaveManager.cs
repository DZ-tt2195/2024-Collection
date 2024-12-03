using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Week1
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] List<BaseEnemy> listOfEnemies = new();

        private void Awake()
        {
            listOfEnemies = FindObjectsByType<BaseEnemy>(FindObjectsSortMode.None).ToList();
        }

        private void Update()
        {
            listOfEnemies.RemoveAll(enemy => enemy == null);
            foreach (BaseEnemy enemy in listOfEnemies)
            {
                if (enemy.health != 0)
                    return;
            }
            Debug.Log("wave cleared");
        }
    }
}