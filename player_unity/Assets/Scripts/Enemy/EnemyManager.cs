using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [Header("Spawn configuration")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint_seed;
    [SerializeField, Min(1)] public int num_enemies = 4;
    [SerializeField, Min(0f)] private float spawnDistance = 3f;
    [SerializeField] public bool spawn_near_player;

    // Kept public for compatibility with existing scripts.
    public bool[] enemyDead;
    public GameObject[] enemies;
    [SerializeField] private Transform[] spawnPoints;

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

    private void Start()
    {
        enemyDead = new bool[num_enemies];
        enemies = new GameObject[num_enemies];
        spawnPoints = new Transform[num_enemies];

        for (int i = 0; i < num_enemies; i++)
        {
            enemyDead[i] = true;
            GameObject point = new GameObject($"EnemySpawnPoint{i + 1}");
            point.transform.SetParent(transform);
            spawnPoints[i] = point.transform;
        }

        if (spawnPoint_seed == null)
        {
            GameObject seed = new GameObject("EnemySpawnPointSeed");
            seed.transform.SetParent(transform);
            spawnPoint_seed = seed.transform;
        }

        SetInitialSpawnSeed();
        RecalculateSpawnPoints();
        HandleSpawn();
    }

    private void Update()
    {
        HandleSpawn();
    }

    private void SetInitialSpawnSeed()
    {
        if (spawn_near_player)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                spawnPoint_seed.position = player.transform.position + Vector3.left * 5f;
                return;
            }
            Debug.LogWarning("EnemyManager: Player not found; using world origin for spawn seed.", this);
        }
        else
        {
            GameObject seedObject = GameObject.Find("enemy_spawn_seed");
            if (seedObject != null)
            {
                spawnPoint_seed.position = seedObject.transform.position;
                return;
            }
            Debug.LogWarning("EnemyManager: enemy_spawn_seed not found; using world origin.", this);
        }

        spawnPoint_seed.position = Vector3.zero;
    }

    public void RecalculateSpawnPoints()
    {
        if (spawnPoint_seed == null || spawnPoints == null)
            return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float angle = i * Mathf.PI * 2f / spawnPoints.Length;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnDistance;
            spawnPoints[i].position = spawnPoint_seed.position + offset;
        }
    }

    public void HandleSpawn()
    {
        if (enemyPrefab == null || enemyDead == null || enemies == null)
            return;

        for (int i = 0; i < enemyDead.Length; i++)
        {
            if (!enemyDead[i])
                continue;

            GameObject enemy = SpawnEnemy(spawnPoints[i]);
            if (enemy == null)
                continue;

            enemies[i] = enemy;
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
                health.enemy_idx = i; // Always match the actual spawn slot.
            else
                Debug.LogWarning("Enemy prefab is missing EnemyHealth.", enemy);

            enemyDead[i] = false;
        }
    }

    public void NotifyEnemyDied(int index)
    {
        if (enemyDead == null || index < 0 || index >= enemyDead.Length)
            return;

        enemyDead[index] = true;
        enemies[index] = null;
    }

    public GameObject SpawnEnemy(Transform spawnPoint)
    {
        if (enemyPrefab == null || spawnPoint == null)
            return null;

        return Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
