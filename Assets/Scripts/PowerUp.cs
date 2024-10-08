using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] string m_PowerupName;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement _movement = collision.gameObject.GetComponent<PlayerMovement>();
            _movement.PowerUp(m_PowerupName);
            Destroy(gameObject);
        }
    }
}
