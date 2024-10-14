using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] int m_DamagePoints;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerSettings.Instance.playerHP -= m_DamagePoints;
            Instantiate(PlayerSettings.Instance.explotion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Rigidbody2D _cRb = collision.gameObject.GetComponent<Rigidbody2D>();
            _cRb.velocity = new Vector2(0, 0);
        }
    }
}
