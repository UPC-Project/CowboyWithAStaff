using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup _background;
    [SerializeField] private GameObject _NeedKeyText;
    
    [SerializeField] private PlayerInput _playerInput;


    private void Start()
    {
        _playerInput = Player.Instance.GetComponent<PlayerInput>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Player.Instance.hasGraveyardKey)
            {
                Player.Instance.audioSourceWalk.mute = true;
               
                Time.timeScale = 0f;
                _playerInput.DeactivateInput(); // Disable player input
                StartCoroutine(EnterBossFight());
            }
            else
            {
                _NeedKeyText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _NeedKeyText.SetActive(false);
    }

    private IEnumerator EnterBossFight()
    {
        StartCoroutine(Animations.Instance.FadeIn(v => _background.alpha = v));
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("BossFight");
        Time.timeScale = 1f;
    }
}
