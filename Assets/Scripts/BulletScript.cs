using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D m_rb;
    [SerializeField] float m_shootForce;
    bool m_canDamage = false;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

        m_rb.AddForce(Vector2.up * m_shootForce, ForceMode2D.Force);

        Destroy(gameObject, 5);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_canDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_canDamage)
        {
            Destroy(gameObject);

            HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
            if(_hitPoints != null)
            {
                _hitPoints.m_HP--;
            }
            else
            {
                Debug.Log("Object does not have HitPoints script");
            }
        }
    }
}
