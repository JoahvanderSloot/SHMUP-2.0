using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] int m_DamagePoints;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if you hit the player and damage it when you do
        if (collision.gameObject.CompareTag("Player"))
        {
            GameInfoSingleton.Instance.playerSettings.playerHP -= m_DamagePoints;
            Instantiate(GameInfoSingleton.Instance.playerSettings.explotion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        // Check if you hit a object with the same tag as you and ignore the collider so you can fly trough it next time
        // Set the velocity of the object you bumped to 0 so it does not go flying
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Rigidbody2D _cRb = collision.gameObject.GetComponent<Rigidbody2D>();
            _cRb.velocity = new Vector2(0, 0);
        }
    }
}
