using System.Collections;
using UnityEngine;

public class ShootingEnemy : EnemyBase
{
    [SerializeField] GameObject m_bullet;
    [SerializeField] float m_ShootTimer = 3;
    [SerializeField] float m_enemyShootForce;

    private Coroutine currentShootCoroutine;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (m_isInPosition)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);

            if (m_enemyType == 0 && currentShootCoroutine == null)
            {
                currentShootCoroutine = StartCoroutine(ShootDown());
            }
            else if (m_enemyType == 1 && currentShootCoroutine == null)
            {
                currentShootCoroutine = StartCoroutine(ShootAtPlayer());
            }
        }
    }

    IEnumerator ShootDown()
    {
        while (true)
        {
            GameObject _instantedBullet = Instantiate(m_bullet, transform.position, Quaternion.identity);
            BulletScript _enemyBullet = _instantedBullet.GetComponent<BulletScript>();
            _enemyBullet.m_shootForce = m_enemyShootForce;
            _enemyBullet.m_ShootDirection = Vector2.down;
            yield return new WaitForSeconds(m_ShootTimer);
        }
    }

    IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            GameObject _instantedBullet = Instantiate(m_bullet, transform.position, Quaternion.identity);
            BulletScript _enemyBullet = _instantedBullet.GetComponent<BulletScript>();
            _enemyBullet.m_shootForce = m_enemyShootForce;
            yield return new WaitForSeconds(m_ShootTimer);
        }
    }
}
