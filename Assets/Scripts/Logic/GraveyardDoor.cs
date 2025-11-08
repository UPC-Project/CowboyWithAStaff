using UnityEngine;

public class GraveyardDoor : MonoBehaviour
{
    private BossFight _bossFight;
    private void Start()
    {
        _bossFight = FindAnyObjectByType<BossFight>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(_bossFight.EnterBossFight()); 
    }
}
