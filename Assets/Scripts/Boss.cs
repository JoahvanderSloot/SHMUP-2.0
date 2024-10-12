using System.Collections;
using UnityEngine;

public class Boss : EnemyBase
{
    [SerializeField] float m_newMovementPhaseTimer;
    [SerializeField] int m_currentMovementPhase;
    [SerializeField] GameObject m_bullet;
    Vector2 m_moveDirection;

    Coroutine m_attackCoroutine;

    protected override void Start()
    {
        m_enemyType = 0;

        base.Start();

        m_hitPoints.m_HP = (4 * PlayerSettings.Instance.wave) + 12;

        m_moveDirection = new Vector2(Random.value > 0.5f ? 1 : -1, 0);
        m_Speed += (m_floatWave / 4);
    }

    protected override void Update()
    {
        base.Update();
        if(m_attackCoroutine == null && m_isInPosition)
        {
            m_attackCoroutine = StartCoroutine(BossAttack(500));
        }
        if(m_isInPosition)
        {
            BossMovement();
        }
        BossRotation();
    }

    IEnumerator BossAttack(float _shootForce)
    {
        while (true)
        {
            GameObject _instantedBullet = Instantiate(m_bullet, transform.position, transform.rotation);
            _instantedBullet.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            BulletScript _enemyBullet = _instantedBullet.GetComponent<BulletScript>();
            _enemyBullet.m_shootForce = _shootForce;
            _enemyBullet.m_ShootDirection = transform.up;
            yield return new WaitForSeconds(1);
        }
    }

    private void BossRotation()
    {
        GameObject _player = GameObject.FindWithTag("Player");
        Vector3 _relativePos = _player.transform.position;
        Quaternion _lookAt = Quaternion.LookRotation(Vector3.forward, _relativePos - transform.position);
        transform.rotation = _lookAt;
    }

    private void BossMovement()
    {
        float _rightSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x - 0.15f;
        float _leftSideOfScreen = -_rightSideOfScreen + 0.15f;

        if (transform.position.x > _rightSideOfScreen)
        {
            m_moveDirection = Vector2.left;
        }
        if (transform.position.x < _leftSideOfScreen)
        {
            m_moveDirection = Vector2.right;
        }

        m_rb.AddForce(m_moveDirection.normalized * m_Speed * Time.deltaTime * 1000, ForceMode2D.Force);
    }
}
