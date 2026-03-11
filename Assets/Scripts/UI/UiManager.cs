using Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Image[] _heartsCanvas;
    [SerializeField] private Image _ranged_cooldown;
    [SerializeField] private Image _melee_cooldown;
    [SerializeField] private Image _block_cooldown;
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
        _ranged_cooldown.fillAmount = 0;
        _melee_cooldown.fillAmount = 0;
        _block_cooldown.fillAmount = 0;
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
            _ranged_cooldown.fillAmount = fill;

        } else if (skill == PlayerSkill.MeleeAttack) {

            _melee_cooldown.fillAmount = fill;
        } else 
        {
            _block_cooldown.fillAmount = fill;
        }
    }
}