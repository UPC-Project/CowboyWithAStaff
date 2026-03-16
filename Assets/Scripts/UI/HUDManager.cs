using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] private Image[] _heartsCanvas;
    [SerializeField] private Image _rangedCooldown;
    [SerializeField] private Image _meleeCooldown;
    [SerializeField] private Image _blockCooldown;
    [SerializeField] private TextMeshProUGUI _potionText;
    [SerializeField] private TextMeshProUGUI _keyText;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject); // Persist across scenes
    }

    private void Start()
    {
        _rangedCooldown.fillAmount = 0;
        _meleeCooldown.fillAmount = 0;
        _blockCooldown.fillAmount = 0;
    }

    public void UpdateHeartsUI(int health)
    {
        for (int i = 0; i < _heartsCanvas.Length; i++)
        {
            if (i < health)
            {
                _heartsCanvas[i].enabled = true;
            }
            else
            {
                _heartsCanvas[i].enabled = false;
            }
        }
    }
    public void UpdatePotionText(string potions)
    {
        _potionText.text = potions;
    }

    public void UpdateKey(bool hasKey)
    {
        _keyText.text = hasKey ? "1" : "0";
    }

    public void UpdateSkills(PlayerSkill skill, float fill)
    {
        if (skill == PlayerSkill.RangedAttack)
        {
            _rangedCooldown.fillAmount = fill;

        } else if (skill == PlayerSkill.MeleeAttack) {

            _meleeCooldown.fillAmount = fill;
        } else 
        {
            _blockCooldown.fillAmount = fill;
        }
    }
}