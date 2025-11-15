using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SheriffRoomManager : MonoBehaviour
{

    //room states
    private enum RoomState { Idle, Fighting, Completed }
    //base state
    private RoomState _currentState = RoomState.Idle;

    [Header("Setup")]
    [SerializeField] private GameObject _entryTriggerObject;
    [SerializeField] private SheriffRoomTrigger _entryTrigger;
    [SerializeField] private Transform _playerTeleportTarget;
    [SerializeField] private Transform _exitTeleport;
    [SerializeField] private GameObject _exitTriggerObject;

    [Header("Waves")]
    [SerializeField] private Wave[] _wave;
    private int _currentWaveIndex = 0;
    private List<Health> _activeEnemies = new List<Health>();

    [Header("Rewards")]
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Transform _keySpawnPoint;

    public void Start()
    {
        _exitTriggerObject.gameObject.SetActive(false);
    }

    public void StartRoom(GameObject player)
    {

        if (_currentState != RoomState.Idle) return;
        StartCoroutine(RoomSequence(player));
        Player playerScript = player.GetComponent<Player>();
        playerScript.OnPlayerDied += OnPlayerDied;

    }

    private IEnumerator RoomSequence(GameObject player)
    {
        _currentState = RoomState.Fighting;
        player.transform.position = _playerTeleportTarget.position;
        StartNextWave();

        yield return null;
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

            enemyScript.isManagedByRoom = true;

            enemyScript.OnEnemyDied += OnEnemyDied;
            _activeEnemies.Add(enemyScript);
            enemyScript.ForceAggro();

            yield return new WaitForSeconds(wave.delayBetweenSpawns);
        }
    }

    private void OnEnemyDied(Health deadEnemy)
    {
        Enemy enemyScript = deadEnemy.GetComponent<Enemy>();
        enemyScript.OnEnemyDied -= OnEnemyDied;
        _activeEnemies.Remove(deadEnemy);

        if (_activeEnemies.Count == 0)
        {
            _currentWaveIndex++;
            StartNextWave();
        }

    }

    private void OnPlayerDied()
    {
        Player playerScript = FindAnyObjectByType<Player>();
        playerScript.OnPlayerDied -= OnPlayerDied;
        foreach (Health enemy in _activeEnemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.OnEnemyDied -= OnEnemyDied;
            Destroy(enemy.gameObject);
        }
        _activeEnemies.Clear();
        _currentState = RoomState.Idle;
        _currentWaveIndex = 0;
        _entryTrigger.ResetTrigger();
        _exitTriggerObject.SetActive(false);
    }

    private void CompleteRoom()
    {
        _currentState = RoomState.Completed;
        Instantiate(_keyPrefab, _keySpawnPoint.position, Quaternion.identity);
        _entryTriggerObject.SetActive(false);
        _exitTriggerObject.SetActive(true);
    }

}