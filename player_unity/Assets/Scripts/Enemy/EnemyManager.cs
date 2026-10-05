using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance {get; private set;}


    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;

    public bool enemyDead = false;

    private GameObject playerObject;

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

        spawnPoint = new GameObject("EnemySpawnPoint").transform;

	playerObject = GameObject.FindGameObjectWithTag("Player");
	if (playerObject != null)
	{
	    Debug.Log("Found player, setting spawnPoint to player position.");
            //Set respawn point based on Player's transform.
	    Vector3 pos = playerObject.transform.position;
	    pos.x = pos.x - 5.0f;
	    spawnPoint.position = pos;

	}
	else
	{
	    Debug.Log("Didn't find player, defaulting to 0.");
	    //Default to (0,0)
	    spawnPoint.position = new Vector3(0, 0, 0);

	}
    }

    public void Update()
    {
        if (enemyDead == true)
	{
	    Debug.Log("EnemyManager: enemyDead = true. Spawning new enemy.");
            SpawnEnemy();
	    enemyDead = false;
	}
    }

    public GameObject SpawnEnemy()
    {
        return Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    
}
