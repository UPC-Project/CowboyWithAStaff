using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _winGameCanvas;
    [SerializeField] private TextMeshProUGUI _textWinGame;
    [SerializeField] private CanvasGroup _buttonsWinGame;
    [SerializeField] private GameObject _HUD;

    private void Start()
    {
        _HUD = Menus.Instance.gameObject;
    }
    public IEnumerator WinGame()
    {
        _HUD.SetActive(false);
        Time.timeScale = 0f;
        _camera.SetActive(true);
        _winGameCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        _textWinGame.gameObject.SetActive(true);
        StartCoroutine(Animations.Instance.FadeIn(v => _textWinGame.alpha = v));
        yield return new WaitForSecondsRealtime(3f);
        _buttonsWinGame.gameObject.SetActive(true);
        StartCoroutine(Animations.Instance.FadeIn(v => _buttonsWinGame.alpha = v));
    }

    // TODO: button functionality when UI is merged
    public void GoToMainMenu() // from pause menu
    {
        SceneManager.LoadScene("InGame");
        SceneManager.LoadScene("Menus");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
