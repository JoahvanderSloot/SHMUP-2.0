using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D m_rb;
    public float m_shootForce;
    bool m_canDamage = false;
    public Vector2 m_ShootDirection;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

        m_rb.AddForce(m_ShootDirection * m_shootForce, ForceMode2D.Force);

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
            HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
            if(_hitPoints != null)
            {
                _hitPoints.m_HP--;
            }
            else
            {
                Debug.Log("Object does not have HitPoints script");
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerSettings.Instance.playerHP--;
            }
            Destroy(gameObject);
        }
    }
}
