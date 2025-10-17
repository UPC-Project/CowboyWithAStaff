using UnityEngine;
using UnityEngine.SceneManagement; 
public class Menus : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _mainMenu;

    private bool _isPaused = false;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //&& !_mainMenu.activeSelf
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        _isPaused = true;
        _pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        Debug.Log("Game Resumed");
    }

    public void OptionQuitGame()
    {
        SceneManager.LoadScene("Menus");
    }

}