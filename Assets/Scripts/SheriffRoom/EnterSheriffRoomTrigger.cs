using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnterSheriffRoomTrigger : MonoBehaviour
{
    [SerializeField] private SheriffRoomManager _roomManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((SheriffRoomManager.Instance._currentState != SheriffRoomManager.RoomState.Completed) && collision.CompareTag("Player"))
        {
            _roomManager.StartRoom();
        }
    }
}
