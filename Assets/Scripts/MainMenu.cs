using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("inGame");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}