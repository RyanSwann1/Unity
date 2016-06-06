using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Coord
    {
        public int m_x;
        public int m_y;
        public Coord(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public static bool operator ==(Coord v1, Coord v2)
        {
            return v1.m_x == v2.m_x && v1.m_y == v2.m_y;
        }

        public static bool operator !=(Coord v1, Coord v2)
        {
            return v1.m_x != v2.m_x && v1.m_y != v2.m_y;
        }
    }

    [System.Serializable]
    public struct Map
    {
        public Coord m_mapSize;
        [Range(0, 1)]
        public float m_obstaclePercent;
        [Range(0, 1)]
        public float m_tileBorderSize;
        public float m_tileSize;
        public float m_obstacleMinHeight;
        public float m_obstacleMaxHeight;
        public Color m_backgroundColor;
        public Color m_foregroundColor;
        public int m_seed;

        public Coord GetMapCentre()
        {
            return new Coord(m_mapSize.m_x / 2, m_mapSize.m_y / 2);
        }
        
    }

    public Map[] m_maps;
    public Transform m_tile;
    public Transform m_obstacle;
    public Transform m_navMeshFloor;
    public Transform m_navMeshWall;
    public Coord m_maxMapSize;
    public int m_currentMapIndex;

    private List<Coord> m_allTileCoords;
    private List<Coord> m_allOpenTileCoords;
    private Queue<Coord> m_shuffledObstacleCoords;
    private Queue<Coord> m_shuffledOpenTileCoords;
    private Coord m_mapCentre;
    private Map m_currentMap;
    private bool[,] m_obstacleMap;
    private System.Random m_rand;
    private Transform[,] m_tileMap;
    private Coord m_tileSpawnedEnemy; //Most recent tile that the enemy spawned on

    private void Awake()
    {
    }

    private void Start()
    {
        GenerateMap();
    }
    
    public void GenerateMap()
    {
        LoadMap();

        //If current map is created, destroy
        string mapHolder = "Generated Map";
        if (transform.FindChild(mapHolder))
        {
            DestroyImmediate(transform.FindChild(mapHolder).gameObject);
        }

        //Generate new map
        Transform newMap = new GameObject(mapHolder).transform;
        newMap.parent = transform;

        m_tileMap = new Transform[m_currentMap.m_mapSize.m_x, m_currentMap.m_mapSize.m_y];
        AssignAllTiles();
        GenerateNavMeshFloor(newMap);
        GenerateNavMeshWalls(newMap);
        GenerateTiles(newMap);
        m_obstacleMap = new bool[m_currentMap.m_mapSize.m_x, m_currentMap.m_mapSize.m_y];
        m_mapCentre = new Coord(m_currentMap.m_mapSize.m_x / 2, m_currentMap.m_mapSize.m_y / 2);
        m_allOpenTileCoords = new List<Coord>(m_allTileCoords);
        m_shuffledObstacleCoords = new Queue<Coord>(Utility.ShuffleArray(m_allTileCoords.ToArray(), m_currentMap.m_seed));
        m_shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(m_allOpenTileCoords.ToArray(), m_currentMap.m_seed));
        m_rand = new System.Random(m_currentMap.m_seed);
        GenerateObstacles(newMap);
    }

    public Transform getRandomOpenTile()
    {
        Coord randCoord = m_shuffledOpenTileCoords.Dequeue();
        m_shuffledOpenTileCoords.Enqueue(randCoord);
        return m_tileMap[randCoord.m_x, randCoord.m_y];
    }

    private void GenerateTiles(Transform newMap)
    {
        for (int x = 0; x < m_currentMap.m_mapSize.m_x; x++)
        {
            for (int y = 0; y < m_currentMap.m_mapSize.m_y; y++)
            {
                Vector3 spawnPos = GetTile(x, y);
                Transform newTile = Instantiate(m_tile, spawnPos, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - m_currentMap.m_tileBorderSize) * m_currentMap.m_tileSize;
                
                newTile.parent = newMap;
                m_tileMap[x, y] = newTile;
            }
        }
    }

    private void GenerateObstacles(Transform newMap)
    {
        int obstacleCount = (int)((m_currentMap.m_mapSize.m_x * m_currentMap.m_mapSize.m_y) * m_currentMap.m_obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randCoord = getRandomCoord();
            m_obstacleMap[randCoord.m_x, randCoord.m_y] = true;
            currentObstacleCount++;
            //Obstacle allowed to spawn
            if (randCoord != m_mapCentre && IsMapFullyAccessible(currentObstacleCount))
            {
                SpawnObstacle(newMap, randCoord);
                m_allOpenTileCoords.Remove(randCoord); //Remove so enemy cannot spawn on obstacle
            }
            //Path blocked, obstacle cannot spawn
            else
            {
                m_obstacleMap[randCoord.m_x, randCoord.m_y] = false;
                currentObstacleCount--;
            }
        }
    }

    private void SpawnObstacle(Transform newMap, Coord randCoord)
    {
        float obstacleHeight = Mathf.Lerp(m_currentMap.m_obstacleMinHeight, m_currentMap.m_obstacleMaxHeight, (float)m_rand.NextDouble());

        Vector3 spawnPos = GetTile(randCoord.m_x, randCoord.m_y);
        Transform newObstacle = Instantiate(m_obstacle, spawnPos + Vector3.up * (obstacleHeight / 2), Quaternion.identity) as Transform;
        newObstacle.localScale = new Vector3((1 - m_currentMap.m_tileBorderSize) * m_currentMap.m_tileSize, obstacleHeight, (1 - m_currentMap.m_tileBorderSize) * m_currentMap.m_tileSize);
        newObstacle.parent = newMap;

        //Change colour of obstacle
        Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
        Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
        float colorPercent = randCoord.m_y / (float)m_currentMap.m_mapSize.m_y;
        obstacleMaterial.color = Color.Lerp(m_currentMap.m_foregroundColor, m_currentMap.m_backgroundColor, colorPercent);
        obstacleRenderer.sharedMaterial = obstacleMaterial;


    }

    private bool IsMapFullyAccessible(int currentObstacleCount)
    {
        bool[,] checkedTiles = new bool[m_obstacleMap.GetLength(0), m_obstacleMap.GetLength(1)];
        Queue<Coord> tileQueue = new Queue<Coord>();
        tileQueue.Enqueue(new Coord(m_mapCentre.m_x, m_mapCentre.m_y));
        checkedTiles[m_mapCentre.m_x, m_mapCentre.m_y] = true;
        int currentAccessibleTiles = 1;

        while (tileQueue.Count > 0)
        {
            Coord tile = tileQueue.Dequeue();
            //8 tile radius around checked tile
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourTileX = tile.m_x + x;
                    int neighbourTileY = tile.m_y + y;

                    //Vertical/Horizontal tiles only
                    if (x == 0 || y == 0)
                    {
                        //Make sure neighbouring tiles are within the bounds of the game
                        if (neighbourTileX >= 0 && neighbourTileX < m_obstacleMap.GetLength(0) && neighbourTileY >= 0 && neighbourTileY < m_obstacleMap.GetLength(1))
                        {
                            //Make sure tile not checked and isnt obstacle
                            if (!checkedTiles[neighbourTileX, neighbourTileY] && !m_obstacleMap[neighbourTileX, neighbourTileY])
                            {
                                tileQueue.Enqueue(new Coord(neighbourTileX, neighbourTileY));
                                checkedTiles[neighbourTileX, neighbourTileY] = true;
                                currentAccessibleTiles++;
                            }

                        }
                    }

                }
            }
        }

        int targetAccessibleTiles = (int)((m_currentMap.m_mapSize.m_x * m_currentMap.m_mapSize.m_y - currentObstacleCount));
        return currentAccessibleTiles == targetAccessibleTiles;
    }


    private Vector3 GetTile(int x, int y)
    {
        return new Vector3(-m_currentMap.m_mapSize.m_x / 2f + 0.5f + x, 0, -m_currentMap.m_mapSize.m_y / 2f + 0.5f + y) * m_currentMap.m_tileSize;
    }

    private Coord getRandomCoord()
    {
        Coord randCoord = m_shuffledObstacleCoords.Dequeue();
        m_shuffledObstacleCoords.Enqueue(randCoord);
        return randCoord;
    }






    private void AssignAllTiles()
    {
        m_allTileCoords = new List<Coord>();
        for (int x = 0; x < m_currentMap.m_mapSize.m_x; x++)
        {
            for (int y = 0; y < m_currentMap.m_mapSize.m_y; y++)
            {
                m_allTileCoords.Add(new Coord(x, y));
            }
        }
    }

    private void LoadMap()
    {
        //If map exists
        if (m_currentMapIndex < m_maps.Length)
        {
            //if selected map is within bounds of the maximum allowed map size
            if (m_maps[m_currentMapIndex].m_mapSize.m_x <= m_maxMapSize.m_x && m_maps[m_currentMapIndex].m_mapSize.m_y <= m_maxMapSize.m_y)
            {
                m_currentMap = m_maps[m_currentMapIndex];
            }
            //Change to allowed map size
            else
            {
                m_maps[m_currentMapIndex].m_mapSize.m_x = m_maxMapSize.m_x;
                m_maps[m_currentMapIndex].m_mapSize.m_y = m_maxMapSize.m_y;
                m_currentMap = m_maps[m_currentMapIndex];
            }
        }
    }

    private void GenerateNavMeshFloor(Transform newMap)
    {
        Transform navMeshFloor = Instantiate(m_navMeshFloor, Vector3.zero, Quaternion.identity) as Transform;
        navMeshFloor.localScale = new Vector3(m_currentMap.m_mapSize.m_x / 2, 1, m_currentMap.m_mapSize.m_y / 2) * m_currentMap.m_tileSize / 4;
        navMeshFloor.parent = newMap;
    }

    private void GenerateNavMeshWalls(Transform newMap)
    {
        
        float xAxisSpawnPos = m_currentMap.m_mapSize.m_x / 2f + 0.5f;
        //Spawn left wall
        Transform leftWall = Instantiate(m_navMeshWall, Vector3.left * xAxisSpawnPos * m_currentMap.m_tileSize , Quaternion.identity) as Transform;
        leftWall.localScale = new Vector3(1, 1, m_currentMap.m_mapSize.m_y) * m_currentMap.m_tileSize;
        leftWall.parent = newMap;

        //Spawn the right wall
        Transform rightWall = Instantiate(m_navMeshWall, Vector3.right * xAxisSpawnPos * m_currentMap.m_tileSize, Quaternion.identity) as Transform;
        rightWall.localScale = new Vector3(1, 1, m_currentMap.m_mapSize.m_y) * m_currentMap.m_tileSize;
        rightWall.parent = newMap;
        

        float yAxisSpawnpos = m_currentMap.m_mapSize.m_y / 2f + 0.5f;
        //Spawn top wall
        Transform topWall = Instantiate(m_navMeshWall, Vector3.forward * yAxisSpawnpos * m_currentMap.m_tileSize, Quaternion.identity) as Transform;
        topWall.localScale = new Vector3(m_currentMap.m_mapSize.m_x, 1, 1) * m_currentMap.m_tileSize;
        topWall.parent = newMap;
        //Spawn the Bottom Wall
        Transform botWall = Instantiate(m_navMeshWall, Vector3.back * yAxisSpawnpos * m_currentMap.m_tileSize, Quaternion.identity) as Transform;
        botWall.localScale = new Vector3(m_currentMap.m_mapSize.m_x, 1, 1) * m_currentMap.m_tileSize;
        botWall.parent = newMap;
    }

}







