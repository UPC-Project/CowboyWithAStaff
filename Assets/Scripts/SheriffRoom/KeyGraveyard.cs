using UnityEngine;
using TMPro;

public class KeyGraveyard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.Instance.Play("GetKey");
        Player.Instance.hasGraveyardKey = true;
        UIManager.Instance.UpdateKey(Player.Instance.hasGraveyardKey);
        Destroy(gameObject);
    }
}
