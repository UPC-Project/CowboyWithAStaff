using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class SheriffRoomTrigger : MonoBehaviour
{
    [SerializeField] private SheriffRoomManager _roomManager;
    private bool _triggered = false;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggered || !collision.CompareTag("Player")) return;
        _triggered = true; 
        _roomManager.StartRoom(collision.gameObject);
    }

    public void ResetTrigger()
    {
        _triggered = false;
    }
}
