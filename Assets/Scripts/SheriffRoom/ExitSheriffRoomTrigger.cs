using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExitSheriffRoomTrigger : MonoBehaviour
{
    [SerializeField] public Transform _exitTeleportTarget;
    [SerializeField] private GameObject _roomTextEnd;
    private GameState _gameState;

    private void Start()
    {
        _gameState = FindAnyObjectByType<GameState>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.hasGraveyardKey)
        {
            collision.transform.position = _exitTeleportTarget.position;
            _roomTextEnd.SetActive(false);
            _gameState.ReactivateFrozenEnemies();
        }
    }
}