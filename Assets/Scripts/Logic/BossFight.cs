using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class BossFight : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    public List<string> cinematicText;
    [SerializeField] private GameObject _EnterBossFightCanvas;
    [SerializeField] public TextMeshProUGUI _textCavnas;

    private void Start() 
    {
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
    }
    public IEnumerator EnterBossFight()
    {
        Time.timeScale = 0f;
        //_playerMovement.canMove = false;
        _EnterBossFightCanvas.SetActive(true);
        foreach (string t in cinematicText)
        {
            _textCavnas.text = t;
            yield return new WaitForSecondsRealtime(3f);
        }
        SceneManager.LoadScene("BossFight");
        Time.timeScale = 1f;
    }
}
