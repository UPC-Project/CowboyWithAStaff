using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 7f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private float _lifeTime = 0.2f;
    private float _lifeTimer;
    
    private string _ownerTag;

    public void SetOwner(string tag)
    {
        _ownerTag = tag;
    }

    private void OnEnable()
    {
        _rb.linearVelocity = transform.up * _bulletSpeed;
        _lifeTimer = _lifeTime;
    }

    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        string hitTag = collision.gameObject.tag;

        // If the bullet hits something with the same tag as its owner, it is deactivated.
        if (hitTag == _ownerTag)
        {
            gameObject.SetActive(false);
        }

        // If the bullet is from the "Player" and hits an "Enemy"
        else if (_ownerTag == "Player" && hitTag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(_damage);
            gameObject.SetActive(false); // Desactivar la bala
        }

        // If the bullet is from the "Enemy" and hits the "Player"
        else if (_ownerTag == "Enemy" && hitTag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(_damage);
            gameObject.SetActive(false); // Desactivar la bala
        }

        gameObject.SetActive(false);
    }


}
