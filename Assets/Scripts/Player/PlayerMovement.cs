using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    // Is not attacking or not dead
    public bool canMove = true;
    public Rigidbody2D rb;
    private PlayerInput _playerInput;
    private Vector2 _input;

    [Header("Mouse Direction")]
    [SerializeField] private GameObject _facingPoint;
    private float distanceFromPlayer = 3f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private Vector2 _lastDirection = Vector2.down;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (canMove)
        {
            // Movement
            _input = _playerInput.actions["Move"].ReadValue<Vector2>();
            _input = _input.normalized;

            // Aiming with mouse
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = fixAngle(angle);
            Quaternion rot = Quaternion.Euler(0f, 0f, angle - 90f);
            _facingPoint.transform.localRotation = rot;
            _facingPoint.transform.localPosition = Quaternion.Euler(0, 0, angle) * new Vector3(distanceFromPlayer, 0, 0);

            // Update animations
            Vector2 animDir;
            if (_input.sqrMagnitude > 0.01f)
            {
                animDir = _input;
                _lastDirection = _input;
            }
            else
            {
                animDir = _lastDirection; // Last walk animation
            }

            // Actualizar Animator
            _animator.SetFloat("horizontal", animDir.x);
            _animator.SetFloat("vertical", animDir.y);
            _animator.SetFloat("speed", _input.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(_input.x * movementSpeed, _input.y * movementSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // See if it's worth it
    //public IEnumerator Stune()
    //{
    //    canMove = false;
    //    _animator.SetFloat("speed", 0f);
    //    yield return new WaitForSeconds(.5f);
    //    canMove = true;
    //}

    // The animation will be only in 8 angles
    private float fixAngle(float angle)
    {
        if (Mathf.Abs(angle) <= 22.5)
        {
            return 0f;
        }
        else if (Mathf.Abs(angle) <= 67.5)
        {
            return 45f * Mathf.Sign(angle);
        }
        else if (Mathf.Abs(angle) <= 112.5)
        {
            return 90f * Mathf.Sign(angle);
        }
        else if (Mathf.Abs(angle) <= 157.5)
        {
            return 135f * Mathf.Sign(angle);
        }
        else
        {
            return 180f;
        }
    }

}
