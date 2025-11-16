using UnityEngine;
using TMPro;

public class KeyGraveyard : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: add to UI
        Player.Instance.hasGraveyardKey = true;
        Player.Instance.AddKey();
        Destroy(gameObject);
    }
}
