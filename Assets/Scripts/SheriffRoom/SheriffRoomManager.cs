using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SheriffRoomManager : MonoBehaviour
{
    public static SheriffRoomManager Instance { get; private set; }
    // Room states
    public enum RoomState { Idle, Fighting, Completed }
    // Base state
    public RoomState _currentState = RoomState.Idle;

    // TODO: Change when Player is Singleton
    private GameState _gameState;

    [Header("Setup")]
    [SerializeField] private Transform _playerTeleportTarget;
    [SerializeField] private Transform _exitTeleport;

    [Header("Waves")]
    [SerializeField] private Wave[] _wave;
    private int _currentWaveIndex = 0;
    private List<Enemy> _activeEnemies = new List<Enemy>();

    [Header("Rewards")]
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Transform _keySpawnPoint;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject); // Persist across scenes
    }

    public void Start()
    {
        _gameState = FindAnyObjectByType<GameState>();
    }

    public void StartRoom()
    {
        if (_currentState != RoomState.Idle) return;
        RoomSequence(Player.Instance.gameObject);
        Player.Instance.OnPlayerDied += OnPlayerDied;
        _gameState.FreezeAllEnemies();
    }

    private void RoomSequence(GameObject player)
    {
        _currentState = RoomState.Fighting;
        player.transform.position = _playerTeleportTarget.position;
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (_currentWaveIndex >= _wave.Length && _activeEnemies.Count == 0)
        {
            CompleteRoom();
            return;
        }

        _activeEnemies.Clear();
        Wave currentWave = _wave[_currentWaveIndex];

        StartCoroutine(SpawnWave(currentWave));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (GameObject enemyPrefab in wave.enemyPrefab)
        {
            Transform spawnPoint = wave.spawnPoints[Random.Range(0, wave.spawnPoints.Length)];
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();

            enemyScript.isManagedBySheriffRoom = true;

            enemyScript.OnEnemyDied += OnEnemyDied;
            _activeEnemies.Add(enemyScript);
            enemyScript.ForceAggro();

            yield return new WaitForSeconds(wave.delayBetweenSpawns);
        }
    }

    private void OnEnemyDied(Enemy deadEnemy)
    {
        deadEnemy.OnEnemyDied -= OnEnemyDied;
        _activeEnemies.Remove(deadEnemy);
        Destroy(deadEnemy.gameObject);

        // If all enemies are defeated, start the next wave
        if (_activeEnemies.Count == 0)
        {
            _currentWaveIndex++;
            StartNextWave();
        }
    }

    private void OnPlayerDied()
    {
        Player.Instance.OnPlayerDied -= OnPlayerDied;
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnEnemyDied -= OnEnemyDied;
            Destroy(enemy.gameObject);
        }
        _activeEnemies.Clear();
        _currentState = RoomState.Idle;
        _currentWaveIndex = 0;
        _gameState.ReactivateFrozenEnemies();
    }

    private void CompleteRoom()
    {
        _currentState = RoomState.Completed;
        Instantiate(_keyPrefab, _keySpawnPoint.position, Quaternion.identity);
    }

}