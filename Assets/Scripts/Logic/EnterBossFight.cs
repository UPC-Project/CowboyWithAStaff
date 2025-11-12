using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class EnterBossFight : MonoBehaviour
{
    public List<string> cinematicText;
    [SerializeField] private GameObject _EnterBossFightCanvas;
    [SerializeField] private TextMeshProUGUI _textEnterBossFight;
    
    public IEnumerator BossFightCinematic()
    {
        Time.timeScale = 0f;
        _EnterBossFightCanvas.SetActive(true);
        foreach (string t in cinematicText)
        {
            _textEnterBossFight.text = t;
            yield return new WaitForSecondsRealtime(3f);
        }
        SceneManager.LoadScene("BossFight");
        Time.timeScale = 1f;
    }
}
