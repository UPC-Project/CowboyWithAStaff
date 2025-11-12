using UnityEngine;

public class GraveyardDoor : MonoBehaviour
{
    private EnterBossFight _bossFight;
    private void Start()
    {
        _bossFight = FindAnyObjectByType<EnterBossFight>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(_bossFight.BossFightCinematic()); 
    }
}
