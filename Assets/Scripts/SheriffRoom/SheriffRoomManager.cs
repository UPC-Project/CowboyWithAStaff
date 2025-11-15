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
    private Player _player;
    private GameState _gameState;

    [Header("Setup")]
    [SerializeField] private GameObject _entryTriggerObject;
    [SerializeField] private EnterSheriffRoomTrigger _entryTrigger;
    [SerializeField] private Transform _playerTeleportTarget;
    [SerializeField] private Transform _exitTeleport;
    [SerializeField] private GameObject _exitTriggerObject;

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
        _exitTriggerObject.gameObject.SetActive(false);
        _player = FindAnyObjectByType<Player>();
        _gameState = FindAnyObjectByType<GameState>();
    }

    public void StartRoom()
    {
        if (_currentState != RoomState.Idle) return;
        RoomSequence(_player.gameObject);
        _player.OnPlayerDied += OnPlayerDied;
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
        if (_currentWaveIndex >= _wave.Length)
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

        // If all enemies are defeated, start the next wave
        if (_activeEnemies.Count == 0)
        {
            _currentWaveIndex++;
            StartNextWave();
        }
    }

    private void OnPlayerDied()
    {
        _player.OnPlayerDied -= OnPlayerDied;
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnEnemyDied -= OnEnemyDied;
            Destroy(enemy.gameObject);
        }
        _activeEnemies.Clear();
        _currentState = RoomState.Idle;
        _currentWaveIndex = 0;
        _exitTriggerObject.SetActive(false);
        _gameState.ReactivateFrozenEnemies();
    }

    private void CompleteRoom()
    {
        _currentState = RoomState.Completed;
        Instantiate(_keyPrefab, _keySpawnPoint.position, Quaternion.identity);
        _entryTriggerObject.SetActive(false);
        _exitTriggerObject.SetActive(true);
    }

}