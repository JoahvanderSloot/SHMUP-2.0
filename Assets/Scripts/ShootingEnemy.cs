using System.Collections;
using UnityEngine;

public class ShootingEnemy : EnemyBase
{
    [SerializeField] GameObject m_bullet;
    [SerializeField] float m_ShootTimer = 3;
    [SerializeField] protected float m_enemyShootForce;
    protected Quaternion m_bulletRotation;

    private Coroutine currentShootCoroutine;
    Vector2 m_shootDirection;

    protected override void Start()
    {
        base.Start();
        m_ShootTimer += m_ShootTimer / (GameInfoSingleton.Instance.playerSettings.wave + 1);
    }

    protected override void Update()
    {
        if(m_ShootTimer <= 1)
        {
            m_ShootTimer = 1;
        }

        base.Update();

        // If the enemy is in the start position it checks what the enem type is
        // When this type is 0 it just sits there and shoots down, but when it is 1 it looks at the player and shoots towards the player
        if (m_isInPosition)
        {
            switch (m_enemyType)
            {
                case 0:
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                        m_bulletRotation = Quaternion.Euler(0, 0, 0);
                        if (currentShootCoroutine == null)
                        {
                            currentShootCoroutine = StartCoroutine(Shoot());
                        }
                        break;
                    }
                case 1:
                    {
                        m_bulletRotation = transform.rotation;

                        GameObject _player = GameObject.FindWithTag("Player");
                        Vector3 _relativePos = _player.transform.position;
                        Quaternion _lookAt = Quaternion.LookRotation(Vector3.forward, _relativePos - transform.position);
                        transform.rotation = _lookAt;

                        if (currentShootCoroutine == null)
                        {
                            currentShootCoroutine = StartCoroutine(Shoot());
                        }
                        break;
                    }
            }
        }
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            // This checks what type the enemy is and sets the shootdirection to ether down or forward (for the enemy thats up because of its rotation)
            // Forward shoots towards the player because the rotation looks at the player
            switch (m_enemyType)
            {
                case 0:
                    {
                        m_shootDirection = Vector2.down;
                        break;
                    }
                case 1:
                    {
                        m_shootDirection = transform.up;
                        break;
                    }
            }
           
            GameObject _instantedBullet = Instantiate(m_bullet, transform.position, m_bulletRotation);
            BulletScript _enemyBullet = _instantedBullet.GetComponent<BulletScript>();
            _enemyBullet.m_shootForce = m_enemyShootForce;
            _enemyBullet.m_ShootDirection = m_shootDirection;
            yield return new WaitForSeconds(m_ShootTimer);
        }
    }
}
