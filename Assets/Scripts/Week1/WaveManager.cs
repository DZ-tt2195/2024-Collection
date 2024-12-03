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
        List<BaseEnemy> allEnemies = new();
        List<Wave> listOfWaves = new();
        List<EnemyStat> listOfEnemies = new();

        [SerializeField] Bullet bulletPrefab;
        [SerializeField] BaseEnemy enemyPrefab;

        [SerializeField] Slider waveSlider;
        [SerializeField] TMP_Text waveCounter;
        int currentWave = -1;

        private void Awake()
        {
            instance = this;

            Wave[] loadedWaves = Resources.LoadAll<Wave>("Week1/Waves");
            foreach (var obj in loadedWaves)
                listOfWaves.Add(obj);
            EnemyStat[] loadedEnemies = Resources.LoadAll<EnemyStat>("Week1/Enemies");
            foreach (var obj in loadedEnemies)
                listOfEnemies.Add(obj);

            NewWave();
        }

        void NewWave()
        {
            currentWave++;
            waveSlider.value = ((currentWave+1) / (float)listOfWaves.Count);
            waveCounter.text = $"Wave {currentWave+1} / {listOfWaves.Count}";

            try
            {
                foreach (Vector2 vector in listOfWaves[currentWave].enemySpawns)
                    CreateEnemy(vector, listOfEnemies[Random.Range(0, listOfEnemies.Count)]);
            }
            catch
            {

            }
        }

        public void CreateEnemy(Vector2 start, EnemyStat stat)
        {
            BaseEnemy enemy = Instantiate(enemyPrefab);
            enemy.EnemySetup(stat);
            enemy.transform.position = start;
            allEnemies.Add(enemy);
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
            allEnemies.RemoveAll(enemy => enemy == null);
            if (allEnemies.Count > 0)
            {
                foreach (BaseEnemy enemy in allEnemies)
                {
                    if (enemy.health != 0)
                        return;
                }
                for (int i = allEnemies.Count - 1; i >= 0; i--)
                {
                    Destroy(allEnemies[i].gameObject);
                }
                allEnemies.Clear();
                NewWave();
            }
        }
    }
}