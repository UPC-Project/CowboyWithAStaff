using UnityEngine;
using UnityEngine.SceneManagement;

public class GraveyardDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("BossFight");
    }
}
