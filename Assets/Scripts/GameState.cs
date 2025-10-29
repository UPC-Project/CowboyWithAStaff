using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public static GameState Instance;
    private Vector2 _respawnPoint;
    private int _respawnHealth;
    private int _respawnHealingPotions;
    private List<GameObject> _collectedItemsSinceCheckpoint = new List<GameObject>();
    private List<GameObject> _defeatedEnemiesSinceCheckpoint = new List<GameObject>();
    private Player _player;

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _player = FindAnyObjectByType<Player>();

        _respawnPoint = _player.transform.position;
        _respawnHealth = _player.health;
        _respawnHealingPotions = _player.healingPotions;
    }

    //for the moment of touching a checkpoint
    public void SetCheckpoint(Vector2 newPosition)
    {
        _respawnPoint = newPosition;
        _respawnHealth = _player.health;
        _respawnHealingPotions = _player.healingPotions;
        _collectedItemsSinceCheckpoint.Clear();
        _defeatedEnemiesSinceCheckpoint.Clear();
    }

    //by the time you respawn
    public void Respawn()
    {
        _player.transform.position = _respawnPoint;
        _player.health = _respawnHealth;
        _player.healingPotions = _respawnHealingPotions;

        Enemy[] allLivingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in allLivingEnemies)
        {
            enemy.ResetEnemyState();
        }

        //to return collectibles to their original position
        foreach (GameObject item in _collectedItemsSinceCheckpoint)
        {
            item.SetActive(true);
        }
        _collectedItemsSinceCheckpoint.Clear();

        //to the reset enemy state
        foreach (GameObject enemy in _defeatedEnemiesSinceCheckpoint)
        {
            enemy.SetActive(true);
            enemy.GetComponent<HealthSystem>().ResetHealth();
            enemy.GetComponent<Enemy>().ResetEnemyState();
        }
        _defeatedEnemiesSinceCheckpoint.Clear();
    }
    public void RegisterDefeatedEnemy(GameObject enemy)
    {
        _defeatedEnemiesSinceCheckpoint.Add(enemy);
    }

    public void RegisterCollectedItem(GameObject item)
    {
        _collectedItemsSinceCheckpoint.Add(item);
    }


}
