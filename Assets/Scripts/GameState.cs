using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    private Vector2 _respawnPoint;
    private int _respawnHealth;
    private int _respawnHealingPotions;
    private List<GameObject> _collectedItemsSinceCheckpoint = new List<GameObject>();
    private List<GameObject> _activatedEnemiesSinceCheckpoint = new List<GameObject>();
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

    // When a Checkpoint is triggered
    public void SetCheckpoint(Vector2 newPosition)
    {
        _respawnPoint = newPosition;
        _respawnHealth = _player.health;
        _respawnHealingPotions = _player.healingPotions;
        _collectedItemsSinceCheckpoint.Clear();
        _activatedEnemiesSinceCheckpoint.Clear();
    }

    // Called by Death()
    public void Respawn()
    {
        _player.transform.position = _respawnPoint;
        _player.health = _respawnHealth;
        _player.healingPotions = _respawnHealingPotions;

        // Return collectables to their original position
        foreach (GameObject item in _collectedItemsSinceCheckpoint)
        {
            item.SetActive(true);
        }
        _collectedItemsSinceCheckpoint.Clear();

        // Resets enemies states
        foreach (GameObject enemy in _activatedEnemiesSinceCheckpoint)
        {
            enemy.GetComponent<Enemy>().ResetEnemyState();
        }
        _activatedEnemiesSinceCheckpoint.Clear();
    }
    public void RegisterActivatedEnemy(GameObject enemy)
    {
        _activatedEnemiesSinceCheckpoint.Add(enemy);
    }

    public void RegisterCollectedItem(GameObject item)
    {
        _collectedItemsSinceCheckpoint.Add(item);
    }
}
