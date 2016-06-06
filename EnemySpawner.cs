using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    [System.Serializable]
    public struct Wave
    {
        public int m_numberOfEnemies;
        public float m_timeBetweenSpawn;
    }

    public Enemy m_enemy;
    public Wave[] m_waves;

    private Wave m_currentWave;
    private MapGenerator m_mapGenerator;
    private Transform m_enemyHolder;
    private int m_currentWaveIndex;
    private float m_nextSpawnTime;
    private int m_enemiesToSpawn;
    private MapGenerator m_map;

    private void Awake()
    {
        m_mapGenerator = GetComponent<MapGenerator>();

        
    }

    private void Start()
    {
        m_currentWaveIndex = 0;
        string enemyHolderName = "Enemies";
        m_enemyHolder = new GameObject(enemyHolderName).transform;
        m_map = FindObjectOfType<MapGenerator>();
        m_nextSpawnTime = 0;
        NextWave();
    }

    private void Update()
    {
        //ChangeTileMaterial();
    }

    public void NextWave()
    {
        m_currentWaveIndex++;
        //Check the next wave exists
        if(m_currentWaveIndex - 1 < m_waves.Length)
        {
            m_currentWave = m_waves[m_currentWaveIndex - 1];
            m_enemiesToSpawn = m_currentWave.m_numberOfEnemies;
            
            StartCoroutine(Spawn());
        }

    }

    IEnumerator Spawn()
    {
        while (m_enemiesToSpawn > 0)
        {
            if (Time.time > m_nextSpawnTime)
            {
                Debug.Log("Spawn");
                Transform spawnTile = m_map.getRandomOpenTile();
                Material tileMaterial = spawnTile.gameObject.GetComponent<Renderer>().material;
                tileMaterial.color = Color.red; //Change color
                m_nextSpawnTime = Time.time + m_currentWave.m_timeBetweenSpawn;
                Enemy newEnemy = Instantiate(m_enemy, spawnTile.position, Quaternion.identity) as Enemy;
                newEnemy.transform.parent = m_enemyHolder;
                m_enemiesToSpawn--;
            }

            if (m_enemiesToSpawn <= 0)
            {
                NextWave();
            }

            yield return new WaitForSeconds(m_currentWave.m_timeBetweenSpawn);
        }
    }

    private void ChangeTileMaterial()
    {
        Transform spawnTile = m_map.getRandomOpenTile();
        Material tileMaterial = spawnTile.gameObject.GetComponent<Renderer>().material;

        tileMaterial.color = Color.red;
    }
}
