using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Coin coinPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float minDistanceBetweenCoins = 1.5f;
    [SerializeField] private int maxSpawnAttempts = 10;
    [SerializeField] private bool useGridSpawning = true;
    [SerializeField] private int gridWidth = 5;
    [SerializeField] private int gridHeight = 4;

    private ObjectPool<PoolableObject> coinPool;
    private Camera mainCamera;
    private Coroutine spawnCoroutine;

    // Cached calculations for performance
    private Vector2 screenBounds;
    private Vector2 spawnPadding = new Vector2(0.1f, 0.1f);

    // Position tracking for overlap prevention
    private List<Vector3> activeCoinPositions = new List<Vector3>();
    private Queue<Vector3> gridPositions = new Queue<Vector3>();
    private HashSet<Vector3> usedGridPositions = new HashSet<Vector3>();

    // Performance optimization
    private WaitForSeconds[] cachedWaitTimes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main;
            CalculateScreenBounds();
            InitializePool();
            CacheWaitTimes();

            if (useGridSpawning)
                GenerateGridPositions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to game events
        GameEvents.Instance.OnGamePaused.AddListener(StopSpawning);
        GameEvents.Instance.OnGameResumed.AddListener(StartSpawning);
        GameEvents.Instance.OnCoinCollected.AddListener(OnCoinCollected);
    }

    private void InitializePool()
    {
        coinPool = new ObjectPool<PoolableObject>(
            coinPrefab as PoolableObject,
            gameConfig.InitialPoolSize,
            gameConfig.MaxPoolSize,
            transform
        );
    }

    private void CalculateScreenBounds()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));
        screenBounds = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    private void CacheWaitTimes()
    {
        // Pre-cache WaitForSeconds objects to avoid GC allocations
        int cacheSize = 20;
        cachedWaitTimes = new WaitForSeconds[cacheSize];

        for (int i = 0; i < cacheSize; i++)
        {
            float waitTime = gameConfig.SpawnInterval.x +
                           (gameConfig.SpawnInterval.y - gameConfig.SpawnInterval.x) * (i / (float)(cacheSize - 1));
            cachedWaitTimes[i] = new WaitForSeconds(waitTime);
        }
    }

    private void GenerateGridPositions()
    {
        gridPositions.Clear();
        usedGridPositions.Clear();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float normalizedX = (x + 0.5f) / gridWidth;
                float normalizedY = (y + 0.5f) / gridHeight;

                // Apply padding
                normalizedX = Mathf.Lerp(spawnPadding.x, 1f - spawnPadding.x, normalizedX);
                normalizedY = Mathf.Lerp(spawnPadding.y, 1f - spawnPadding.y, normalizedY);

                Vector3 viewportPos = new Vector3(normalizedX, normalizedY, 10f);
                Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

                gridPositions.Enqueue(worldPos);
            }
        }
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        while (GameManager.Instance.IsGameActive)
        {
            if (GameStateMachine.Instance.CurrentState == GameState.Playing)
            {
                if (coinPool.ActiveCount < gameConfig.MaxCoinsOnScreen)
                {
                    SpawnCoin();
                }
            }

            // Use cached wait time for better performance
            float waitTime = Random.Range(gameConfig.SpawnInterval.x, gameConfig.SpawnInterval.y);
            int cacheIndex = Mathf.FloorToInt((waitTime - gameConfig.SpawnInterval.x) /
                           (gameConfig.SpawnInterval.y - gameConfig.SpawnInterval.x) * (cachedWaitTimes.Length - 1));
            cacheIndex = Mathf.Clamp(cacheIndex, 0, cachedWaitTimes.Length - 1);

            yield return cachedWaitTimes[cacheIndex];
        }
    }

    private void SpawnCoin()
    {
        var coin = coinPool.Get() as Coin;
        if (coin != null)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                coin.transform.position = spawnPosition;
                coin.Initialize(coinPool);

                // Track position for overlap prevention
                activeCoinPositions.Add(spawnPosition);
            }
            else
            {
                // Return coin to pool if no valid position found
                coinPool.ReturnToPool(coin as PoolableObject);
            }
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        if (useGridSpawning)
        {
            return GetGridSpawnPosition();
        }
        else
        {
            return GetRandomSpawnPosition();
        }
    }

    private Vector3 GetGridSpawnPosition()
    {
        // Refill grid if empty
        if (gridPositions.Count == 0)
        {
            GenerateGridPositions();

            // Remove positions that are still occupied
            var tempQueue = new Queue<Vector3>();
            while (gridPositions.Count > 0)
            {
                Vector3 pos = gridPositions.Dequeue();
                if (!IsPositionTooClose(pos))
                {
                    tempQueue.Enqueue(pos);
                }
            }
            gridPositions = tempQueue;
        }

        if (gridPositions.Count > 0)
        {
            return gridPositions.Dequeue();
        }

        return Vector3.zero; // No valid position available
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float x = Random.Range(spawnPadding.x, 1f - spawnPadding.x);
            float y = Random.Range(spawnPadding.y, 1f - spawnPadding.y);
            Vector3 viewportPos = new Vector3(x, y, 10f);
            Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);

            if (!IsPositionTooClose(worldPos))
            {
                return worldPos;
            }
        }

        return Vector3.zero; // No valid position found after max attempts
    }

    private bool IsPositionTooClose(Vector3 newPosition)
    {
        foreach (Vector3 existingPosition in activeCoinPositions)
        {
            if (Vector3.Distance(newPosition, existingPosition) < minDistanceBetweenCoins)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCoinCollected()
    {
        // Clean up position tracking when coins are collected
        CleanupInactivePositions();
    }

    private void CleanupInactivePositions()
    {
        // Remove positions of coins that no longer exist
        for (int i = activeCoinPositions.Count - 1; i >= 0; i--)
        {
            bool positionHasCoin = false;

            // Check if there's still an active coin at this position
            for (int j = 0; j < transform.childCount; j++)
            {
                Transform child = transform.GetChild(j);
                if (child.gameObject.activeInHierarchy &&
                    Vector3.Distance(child.position, activeCoinPositions[i]) < 0.1f)
                {
                    positionHasCoin = true;
                    break;
                }
            }

            if (!positionHasCoin)
            {
                activeCoinPositions.RemoveAt(i);
            }
        }
    }

    public void ClearAllCoins()
    {
        activeCoinPositions.Clear();
        usedGridPositions.Clear();
        coinPool?.Clear();
    }

    private void OnDestroy()
    {
        coinPool?.Clear();

        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnGamePaused.RemoveListener(StopSpawning);
            GameEvents.Instance.OnGameResumed.RemoveListener(StartSpawning);
            GameEvents.Instance.OnCoinCollected.RemoveListener(OnCoinCollected);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        // Draw spawn bounds
        Gizmos.color = Color.yellow;
        Vector3 center = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        Vector3 size = new Vector3(screenBounds.x * (1f - 2f * spawnPadding.x),
                                  screenBounds.y * (1f - 2f * spawnPadding.y), 0.1f);
        Gizmos.DrawWireCube(center, size);

        // Draw active coin positions
        Gizmos.color = Color.red;
        foreach (Vector3 pos in activeCoinPositions)
        {
            Gizmos.DrawWireSphere(pos, minDistanceBetweenCoins * 0.5f);
        }

        // Draw grid positions if using grid spawning
        if (useGridSpawning)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 pos in gridPositions)
            {
                Gizmos.DrawWireSphere(pos, 0.2f);
            }
        }
    }


    public int ActiveCoinCount => activeCoinPositions.Count;
    public bool IsSpawning => spawnCoroutine != null;
    public float MinDistanceBetweenCoins => minDistanceBetweenCoins;
}