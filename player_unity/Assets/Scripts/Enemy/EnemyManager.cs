using UnityEngine;
using System;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance {get; private set;}


    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint_seed;
    [SerializeField] private Transform[] spawnPoints;
    public bool[] enemyDead;
    public GameObject[] enemies;

    private GameObject playerObject;

    int current_enemy_idx = 0;
    public int num_enemies = 4; //hard-coded to 4 for now
    float spawnDistance = 3;
    [SerializeField] public bool spawn_near_player;

    private void Awake()
    {
	    if (Instance != null && Instance != this)
	    {
	        Destroy(gameObject);
	        return;
	    }

	    Instance = this;
	    DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
	    Debug.Log("Starting EnemyManager");
        
        enemyDead = new bool[4];
        spawnPoints = new Transform[4];
        enemies = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            enemyDead[i] = true;
            spawnPoints[i] = new GameObject($"EnemySpawnPoint{i+1}").transform;
        }

        spawnPoint_seed = new GameObject("EnemySpawnPoint").transform;

    	playerObject = GameObject.FindGameObjectWithTag("Player");
        //If the option to have enemies spawn around the player is checked off, use the player's location to set the spawn seed
        if (spawn_near_player)
        {
	        if (playerObject != null)
	        {
	            Vector3 pos = playerObject.transform.position;
	            pos.x = pos.x - 5.0f;
	            spawnPoint_seed.position = pos;
	            Debug.Log(@$"Found player, setting spawnPoint to player position.
                            Player Position: ({pos.x}, {pos.y})");
                //Set respawn point based on Player's transform.

	        }
	        else
	        {
	            Debug.Log("Didn't find player, defaulting to 0.");
	            //Default to (0,0)
	            spawnPoint_seed.position = new Vector3(0, 0, 0);
	        }
        }
        else
        {
            //If not, use enemy_spawn_seed game object
            spawnPoint_seed.position = GameObject.Find("enemy_spawn_seed").transform.position;
        }
        //Initialize initial spawn points based on the seed

        for (int i = 0; i < 4; i++)
        {
            float px = spawnPoint_seed.position.x;
            float py = spawnPoint_seed.position.y;
            float pz = spawnPoint_seed.position.z;

            //Set point about circle of radius, spawnDistance, on period of pi/2
            px += spawnDistance * (float)Math.Cos(i*3.14/2);
            py += spawnDistance * (float)Math.Sin(i*3.14/2);

            spawnPoints[i].position = new Vector3((float)px, (float)py, (float)pz);
        }

        HandleSpawn();

    }

    public void Update()
    {
        HandleSpawn();
    }

    public void RecalculateSpawnPoints()
    {
        //Initialize spawn points based on the seed
        //This only needs to be called when the spawnPoint_seed changes

        for (int i = 0; i < 4; i++)
        {
            float px = spawnPoint_seed.position.x;
            float py = spawnPoint_seed.position.y;
            float pz = spawnPoint_seed.position.z;

            //Set point about circle of radius, spawnDistance, on period of pi/2
            px += spawnDistance * (float)Math.Cos(i*3.14/2);
            py += spawnDistance * (float)Math.Sin(i*3.14/2);

            Vector3 pos;
            pos.x = px;
            pos.y = py;
            pos.z = pz;

            spawnPoints[i].position = pos;

        }
    
    }

    public void HandleSpawn()
    {
        for (int i = 0; i < 4; i++)
        {
            if (enemyDead[i])
            {
                enemies[i] = SpawnEnemy(spawnPoints[i]);
                enemies[i].GetComponent<EnemyHealth>().enemy_idx = (current_enemy_idx++)%4;
                enemyDead[i] = false;
            }
        }
    }

    public GameObject SpawnEnemy(Transform spawnPoint)
    {
        Debug.Log("SpawnEnemy called");
        return Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
}
