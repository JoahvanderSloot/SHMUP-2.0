using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D m_rb;
    public float m_shootForce;
    bool m_canDamage = false;
    public Vector2 m_ShootDirection;
    public bool m_canDamageEnemy = false;
    public int m_damage = 1;
    [SerializeField] List<Sprite> m_bulletSprites;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

        m_rb.AddForce(m_ShootDirection * m_shootForce, ForceMode2D.Force);

        Destroy(gameObject, 5);
    }

    private void Update()
    {
        gameObject.transform.localScale = new Vector3(2 + m_damage, 2 + m_damage, 2 + m_damage);

        if (m_damage < m_bulletSprites.Count)
        {
            SpriteRenderer _playerSpriteRenderer = GetComponent<SpriteRenderer>();
            _playerSpriteRenderer.sprite = m_bulletSprites[m_damage];
        }

        if(transform.position.y >= 5.5)
        {
            Destroy(gameObject);
        }
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
                _hitPoints.m_HP -= m_damage;
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
            if (!GameInfoSingleton.Instance.playerSettings.shieldIsActive)
            {
                    GameInfoSingleton.Instance.playerSettings.playerHP--;
            }
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            m_rb.velocity /= 2;
        }
    }
}