using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnterBossFight : MonoBehaviour
{
    public List<string> cinematicText;
    [SerializeField] private GameObject _EnterBossFightCanvas;
    [SerializeField] private TextMeshProUGUI _textEnterBossFight;
    private Player _player;

    private void Start()
    {
        _player = FindFirstObjectByType<Player>();
    }

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
        _player.transform.position = new Vector3(0, -15, 0);
        Time.timeScale = 1f;
    }
}
