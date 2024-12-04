using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

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
        [SerializeField] Resupply resupplyPrefab;

        [SerializeField] Slider waveSlider;
        [SerializeField] TMP_Text waveCounter;
        int currentWave = -1;

        Queue<Bullet> bulletQueue = new();
        Queue<Resupply> resupplyQueue = new();

        private void Awake()
        {
            instance = this;

            Wave[] loadedWaves = Resources.LoadAll<Wave>("Week1/Waves");
            foreach (var obj in loadedWaves)
                listOfWaves.Add(obj);
            EnemyStat[] loadedEnemies = Resources.LoadAll<EnemyStat>("Week1/Enemies");
            foreach (var obj in loadedEnemies)
                listOfEnemies.Add(obj);

            InvokeRepeating(nameof(SpawnResupply), 0f, 7.5f);
            NewWave();
        }

        void SpawnResupply()
        {
            Resupply resupply = (resupplyQueue.Count > 0) ? resupplyQueue.Dequeue() : Instantiate(resupplyPrefab);
            resupply.transform.position = new Vector2(Random.Range(-7f, 7f), 4f);
            resupply.gameObject.SetActive(true);
        }

        public void ReturnResupply(Resupply resupply)
        {
            resupplyQueue.Enqueue(resupply);
            resupply.gameObject.SetActive(false);
        }

        void NewWave()
        {
            try
            {
                currentWave++;
                foreach (Vector2 vector in listOfWaves[currentWave].enemySpawns)
                    CreateEnemy(vector, listOfEnemies[Random.Range(0, listOfEnemies.Count)]);
                waveSlider.value = ((currentWave + 1) / (float)listOfWaves.Count);
                waveCounter.text = $"Wave {currentWave + 1} / {listOfWaves.Count}";
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
            Bullet bullet = (bulletQueue.Count > 0) ? bulletQueue.Dequeue() : Instantiate(bulletPrefab);
            bullet.spriteRenderer.color = color;
            bullet.tag = entity.tag;
            bullet.transform.localScale = scale;
            bullet.transform.position = start;
            bullet.direction = direction;
            bullet.gameObject.SetActive(true);
        }

        public void ReturnBullet(Bullet bullet)
        {
            bulletQueue.Enqueue(bullet);
            bullet.gameObject.SetActive(false);
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