using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director; // Timeline director
    [SerializeField] private GameObject _timelineCanvas;
    [SerializeField] private GameObject _playButton;

    private AsyncOperation _inGameScene;

    public void StartGame()
    {
        StarGameCutScene();
    }

    public void PlayGame()
    {
        Debug.Log("Play Game");
        _inGameScene.allowSceneActivation = true;
    }

    private void StarGameCutScene()
    {
        _timelineCanvas.SetActive(true);
        _director.Play();
        _inGameScene = SceneManager.LoadSceneAsync("InGame");
        _inGameScene.allowSceneActivation = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        _director.stopped += OnTimelineStopped;
    }

    private void OnDisable()
    {
        _director.stopped -= OnTimelineStopped;
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        _playButton.SetActive(true);
    }
}