using System.Collections;
using UnityEngine;
using TMPro;

public class WinGameUI : MonoBehaviour
{
    [SerializeField] private GameObject _winGameCanvas;
    [SerializeField] private TextMeshProUGUI _textWinGame;
    [SerializeField] private CanvasGroup _buttonsWinGame;

    public IEnumerator WinGame()
    {
        Time.timeScale = 0f;
        _winGameCanvas.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        _textWinGame.gameObject.SetActive(true);
        StartCoroutine(Animations.Instance.FadeIn(v => _textWinGame.alpha = v));
        yield return new WaitForSecondsRealtime(3f);
        _buttonsWinGame.gameObject.SetActive(true);
        StartCoroutine(Animations.Instance.FadeIn(v => _buttonsWinGame.alpha = v));
    }

    // TODO: button functionality when UI is merged

}
