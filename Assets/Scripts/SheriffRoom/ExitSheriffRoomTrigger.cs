using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExitSheriffRoomTrigger : MonoBehaviour
{
    [SerializeField] public Transform _exitTeleportTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = _exitTeleportTarget.position;
        }
    }
}