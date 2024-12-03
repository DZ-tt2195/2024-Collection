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
        public static WaveManager instance;
        List<BaseEnemy> listOfEnemies = new();
        [SerializeField] List<EnemyStat> possibleEnemies = new();
        [SerializeField] Bullet bulletPrefab;
        [SerializeField] BaseEnemy enemyPrefab;
        int currentWave = 0;

        private void Awake()
        {
            instance = this;
            NewWave();
        }

        void NewWave()
        {
            currentWave++;
            for (int i = 0; i<currentWave*3; i++)
            {
                CreateEnemy(new(Random.Range(-8f, 8f), Random.Range(2.5f, 4.5f)),
                    possibleEnemies[Random.Range(0, possibleEnemies.Count)]);
            }
        }

        public void CreateEnemy(Vector2 start, EnemyStat stat)
        {
            BaseEnemy enemy = Instantiate(enemyPrefab);
            enemy.EnemySetup(stat);
            enemy.transform.position = start;
            listOfEnemies.Add(enemy);
        }

        public void CreateBullet(Entity entity, Color color, Vector3 start, Vector3 scale, Vector3 direction)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.spriteRenderer.color = color;
            bullet.tag = entity.tag;
            bullet.transform.localScale = scale;
            bullet.transform.position = start;
            bullet.direction = direction;
        }

        private void Update()
        {
            listOfEnemies.RemoveAll(enemy => enemy == null);
            if (listOfEnemies.Count > 0)
            {
                foreach (BaseEnemy enemy in listOfEnemies)
                {
                    if (enemy.health != 0)
                        return;
                }
                for (int i = listOfEnemies.Count - 1; i >= 0; i--)
                {
                    Destroy(listOfEnemies[i].gameObject);
                }
                listOfEnemies.Clear();
                NewWave();
            }
        }
    }
}