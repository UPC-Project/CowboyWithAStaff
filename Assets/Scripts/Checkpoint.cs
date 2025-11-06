using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    public Vector3 respawnPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activated)
        {
            activated = true;
            GameState.Instance.SetCheckpoint(respawnPoint);
        }
    }
}