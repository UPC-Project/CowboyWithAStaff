using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    [SerializeField] private Vector2 _respawnPoint;
    private int _respawnHealth;
    private int _respawnHealingPotions;
    private List<GameObject> _collectedItemsSinceCheckpoint = new List<GameObject>();
    [SerializeField] private List<GameObject> _activatedEnemiesSinceCheckpoint = new List<GameObject>();

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        AudioManager.Instance.PlayMusic("LevelMusic");

        _respawnPoint = Player.Instance.transform.position;
        _respawnHealth = Player.Instance.health;
        _respawnHealingPotions = Player.Instance.healingPotions;
    }

    // When a Checkpoint is triggered
    public void SetCheckpoint(Vector2 newPosition)
    {
        _respawnPoint = newPosition;
        _respawnHealth = Player.Instance.health;
        _respawnHealingPotions = Player.Instance.healingPotions;
        _collectedItemsSinceCheckpoint.Clear();

        foreach (GameObject enemy in _activatedEnemiesSinceCheckpoint.ToList())
        {
            // Enemy is already dead and the player has reached a new checkpoint
            if (!enemy.activeSelf)
            {
                _activatedEnemiesSinceCheckpoint.Remove(enemy);
                Destroy(enemy);
            }
            // Any other enemy that has not been killed, will remain on the list
        }
    }

    // Called by Death()
    public void Respawn()
    {
        Player.Instance.transform.position = _respawnPoint;
        Player.Instance.health = _respawnHealth;
        Player.Instance.healingPotions = _respawnHealingPotions;
        UIManager.Instance.UpdateHeartsUI(Player.Instance.health);
        UIManager.Instance.UpdatePotionText(Player.Instance.healingPotions.ToString());
        UIManager.Instance.UpdateKey(Player.Instance.hasGraveyardKey);

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
        if (!_activatedEnemiesSinceCheckpoint.Contains(enemy)) _activatedEnemiesSinceCheckpoint.Add(enemy);
    }

    public void RegisterCollectedItem(GameObject item)
    {
        _collectedItemsSinceCheckpoint.Add(item);
    }

    public void FreezeAllEnemies()
    {
        foreach (GameObject enemy in _activatedEnemiesSinceCheckpoint)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.canMove = false;
        }
    }

    public void ReactivateFrozenEnemies()
    {
        foreach (GameObject enemy in _activatedEnemiesSinceCheckpoint)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.canMove = true;
        }
    }
}
