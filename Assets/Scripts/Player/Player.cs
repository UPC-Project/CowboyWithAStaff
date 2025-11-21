using Constants;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Player : Health
{
    public static Player Instance { get; private set; }

    public int healingPotions = 0;
    [SerializeField] private PlayerMovement _playerMovement;
    public bool inBossFight = false;
    private SpriteRenderer _spriteRenderer;

    [Header("Melee Attack")]
    [SerializeField] private int _meleeAttackDamage = 1;
    [SerializeField] private float _nextMeleeAttackTime;
    [SerializeField] private float _attackMeleeCooldown;
    public float hitRadius;

    [Header("Ranged Attack")]
    [SerializeField] private float _nextRangedAttackTime;
    [SerializeField] private float _attackRangedCooldown;
    [SerializeField] private GameObject _facingPoint;

    [Header("Block Skill")]
    [SerializeField] private float _nextBlockTime;
    [SerializeField] private float _attackBlockCooldown;
    public float repelForce;
    public bool invulnerable = false;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private Vector2 _attackDirection;
    public bool _isDying = false;

    [Header("Ui System")]
    [SerializeField] private Image[] _heartsCanvas;
    [SerializeField] private Image _hitCooldown;
    [SerializeField] private Image _shotCooldown;
    [SerializeField] private Image _blockCooldown;
    [SerializeField] private TextMeshProUGUI _potionText;
    [SerializeField] private TextMeshProUGUI _keyText;

    [Header("Sheriff Room")]
    // TODO: Integration with key will be after merging with boss fight scene
    public bool hasGraveyardKey = false;
    public event Action OnPlayerDied;


    [Header("Sound")]
    public AudioSource audioSource;
    public AudioSource audioSourceWalk;
    [SerializeField] protected List<AudioClip> _idleSounds;
    [SerializeField] protected List<AudioClip> _moveSounds;
    [SerializeField] protected List<AudioClip> _attackSounds;
    [SerializeField] protected List<AudioClip> _damageSounds;

    private void Awake()
    {
        health = maxHealth;

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
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_nextMeleeAttackTime > 0)
        {
            _nextMeleeAttackTime -= Time.deltaTime;
            _hitCooldown.gameObject.SetActive(true);
            _hitCooldown.fillAmount = _nextMeleeAttackTime / _attackMeleeCooldown;
        }
        if (_nextRangedAttackTime > 0)
        {
            _nextRangedAttackTime -= Time.deltaTime;
            _shotCooldown.gameObject.SetActive(true);
            _shotCooldown.fillAmount = _nextRangedAttackTime / _attackRangedCooldown;
        }
        if (_nextBlockTime > 0)
        {
            _nextBlockTime -= Time.deltaTime;
            _blockCooldown.gameObject.SetActive(true);
            _blockCooldown.fillAmount = _nextBlockTime / _attackBlockCooldown;
        }
    }

    // ATTACK
    // Triggered when Q key or LMB is pressed
    public void OnMeleeAttack()
    {
        if (_nextMeleeAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.MeleeAttack.ToString());
            // MeleeAttack() is called in PlayerMeleeAttackStateBehaviour
        }
    }

    // Triggered when E key or RMB is pressed
    public void OnRangedAttack()
    {
        if (_nextRangedAttackTime <= 0 && _playerMovement.canMove)
        {
            AttackAnimation(PlayerSkill.RangedAttack.ToString());
            // RangedAttack() is called in PlayerRangedAttackStateBehaviour
        }
    }

    // Triggered when Space key or MMB is pressed
    public void OnBlockSkill()
    {
        if (_nextBlockTime <= 0 && _playerMovement.canMove)
        {
            // Change when animation is done
            AttackAnimation(PlayerSkill.Block.ToString());
            invulnerable = true;
        }
    }

    public void BlockSkill()
    {
        if (!inBossFight) RepelEnemies();
    }

    private void RepelEnemies()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, hitRadius * 2);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<Enemy>().RepelFromPLayer(transform.position, repelForce);
            }
        }
    }

    private void AttackAnimation(string attack)
    {
        _attackDirection = (_facingPoint.transform.position - transform.position).normalized;

        _animator.SetFloat("horizontal", _attackDirection.x);
        _animator.SetFloat("vertical", _attackDirection.y);
        // The trigger starts the animation and enters to the corresponding attack state
        _animator.SetTrigger(attack);

        _playerMovement.canMove = false;
    }

    // Called by PlayerRangedAttackStateBehaviour/PlayerMeleeAttackStateBehaviour when attack is completed
    public void OnExitAttackState(PlayerSkill playerSkill)
    {
        _playerMovement.canMove = true;
        // Cooldown begins once the animation has finished
        if (playerSkill == PlayerSkill.MeleeAttack)
        {
            _nextMeleeAttackTime = _attackMeleeCooldown;
        }
        else if (playerSkill == PlayerSkill.RangedAttack)
        {
            _nextRangedAttackTime = _attackRangedCooldown;
        }
        else
        {
            invulnerable = false;
            _nextBlockTime = _attackBlockCooldown;
        }
    }

    public void MeleeAttack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(gameObject.transform.position, hitRadius);
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<Enemy>().TakeDamage(_meleeAttackDamage);
            }
        }

        AudioManager.Instance.Play("PlayerMeleeAttack");
    }

    public void RangedAttack()
    {
        // The bullet damage is in the Bullet script
        BulletPool.Instance.RequestBullet(_facingPoint.transform.position, _facingPoint.transform.rotation, "Player");
        AudioManager.Instance.Play("PlayerShoot");
    }

    // DEATH
    public override void StartDeath()
    {
        if (!_isDying)
        {
            _animator.SetTrigger("Death");
            _playerMovement.canMove = false;
            _isDying = true;
        }
    }

    public override void Death()
    {
        OnPlayerDied?.Invoke(); // SheriffRoomManager listens to this event
        GameState.Instance.Respawn();
        _isDying = false;
        _playerMovement.canMove = true;
    }

    // HEALTH
    // Triggered when H key is pressed
    public void OnHeal()
    {
        if (healingPotions > 0)
        {
            AudioManager.Instance.Play("PotionUse");
            health = maxHealth;
            healingPotions -= 1;

            UpdateHeartsUI();
            UpdatePotionText();
        }
    }

    public override void TakeDamage(int damage)
    {
        if (!invulnerable)
        {
            StartCoroutine(EntitiesUtils.FlashInvert(_spriteRenderer, 0.1f));
            health -= damage;
        }
        if (health <= 0) StartDeath();
        UpdateHeartsUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HealingPotion"))
        {
            AudioManager.Instance.Play("PotionPickup");
            healingPotions += 1;
            collision.gameObject.SetActive(false);
            GameState.Instance.RegisterCollectedItem(collision.gameObject);
            UpdatePotionText();
        }
    }


    public void PlayFootstepSound()
    {
        SoundUtils.PlayARandomSound(audioSourceWalk, _moveSounds, 0.5f);
    }

    // Comment this function if you don't want to see the melee range attack
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, hitRadius);
    }

    public void UpdateHeartsUI()
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

    public void UpdatePotionText()
    {
        _potionText.text = healingPotions.ToString();
    }

    public void AddKey()
    {
        _keyText.text = "1";
    }
}
