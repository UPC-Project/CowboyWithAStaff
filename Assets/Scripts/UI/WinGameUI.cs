using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _winGameCanvas;
    [SerializeField] private TextMeshProUGUI _textWinGame;
    [SerializeField] private CanvasGroup _buttonWinGame;

    public IEnumerator WinGame()
    {
        PauseMenu.Instance.gameObject.SetActive(false);
        HUDManager.Instance.gameObject.SetActive(false);
        Time.timeScale = 0f;
        _winGameCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(Animations.Instance.FadeIn(v => _textWinGame.alpha = v));
        yield return new WaitForSecondsRealtime(3f);
        StartCoroutine(Animations.Instance.FadeIn(v => _buttonWinGame.alpha = v));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoMainMenu()
    {
        Destroy(Player.Instance.gameObject);
        Destroy(AudioManager.Instance.gameObject);
        Destroy(HUDManager.Instance.gameObject);
        Destroy(PauseMenu.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

}
