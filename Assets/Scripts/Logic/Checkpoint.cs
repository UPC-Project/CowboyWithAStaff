using UnityEngine;
using UnityEngine.Rendering;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activated)
        {
            activated = true;
            GameState.Instance.SetCheckpoint(transform.position);
        }
    }
}