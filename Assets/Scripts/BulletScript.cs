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

        // Shoot the bullet forward
        m_rb.AddForce(m_ShootDirection * m_shootForce, ForceMode2D.Force);

        Destroy(gameObject, 5);
    }

    private void Update()
    {
        // Set the scale so when the player shoots a heavier bullet it look bigger on screen too
        gameObject.transform.localScale = new Vector3(2 + m_damage, 2 + m_damage, 2 + m_damage);

        // Set the correct spirte for the bullet so it also looks stronger when it deals more damage
        if (m_damage < m_bulletSprites.Count)
        {
            SpriteRenderer _playerSpriteRenderer = GetComponent<SpriteRenderer>();
            _playerSpriteRenderer.sprite = m_bulletSprites[m_damage];
        }

        // Destroys the bullet if it goes out of the top of your screen
        if(transform.position.y >= 5.5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Make it so it only damages when  it has exited a collider because it gets instantated inside a collider of a enemy or the player
        // And i dont want it to hurt the player or the enemy that shoots it
        m_canDamage = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_canDamage && m_canDamageEnemy)
        {
            HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
            if (_hitPoints != null)
            {
                // Check if the object it hits has a HP script and if it does deal damage to the object
                FindObjectOfType<AudioManager>().Play("Hit");
                _hitPoints.m_HP -= m_damage;
            }
            EnemyBase _enemyBase = collision.gameObject.GetComponent<EnemyBase>();
            if (_enemyBase != null)
            {
                // When it hits a enemy it make the enemy flash the damagetick (see EnemyBase for this function)
                _enemyBase.DamageTick();
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && m_canDamage)
        {
            if (!GameInfoSingleton.Instance.playerSettings.shieldIsActive && !m_canDamageEnemy)
            {
                // Damage the player
                FindObjectOfType<AudioManager>().Play("Damage");
                GameInfoSingleton.Instance.playerSettings.playerHP--;
            }
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Make it so when 2 bullets collide the get slower
            m_rb.velocity /= 2;
        }
    }
}