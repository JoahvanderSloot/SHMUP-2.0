using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D m_rb;
    public float m_shootForce;
    bool m_canDamage = false;
    public Vector2 m_ShootDirection;
    public bool m_canDamageEnemy = false;

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
        if (m_canDamage && m_canDamageEnemy)
        {
            HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
            if (_hitPoints != null)
            {
                _hitPoints.m_HP--;
            }
            EnemyBase _enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (_enemyBase != null)
            {
                _enemyBase.DamageTick();
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && m_canDamage)
        {
            PlayerSettings.Instance.playerHP--;
            Destroy(gameObject);
        }
    }
}