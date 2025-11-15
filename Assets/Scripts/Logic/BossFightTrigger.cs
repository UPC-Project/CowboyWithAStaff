using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup _background;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.audioSourceWalk.mute = true;
            Time.timeScale = 0f;
            StartCoroutine(EnterBossFight());
        }
    }

    private IEnumerator EnterBossFight()
    {
        StartCoroutine(Animations.Instance.FadeIn(v => _background.alpha = v));
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("BossFight");
        Time.timeScale = 1f;
    }
}
