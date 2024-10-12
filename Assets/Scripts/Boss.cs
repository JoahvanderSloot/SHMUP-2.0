using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
{
    [SerializeField] GameObject m_bullet;
    Vector2 m_moveDirection;
    [SerializeField] float m_fireRate;
    bool m_isReadyToSpiral;
    bool m_shouldSpiral = true;
    bool m_choseNewPhase = false;
    private Vector3 m_startPos = new Vector3(-1f, 2f, 0f);
    Quaternion m_startRotation;

    Coroutine m_attackCoroutine;
    Coroutine m_phaseSwitchCoroutine;

    private enum bossPhases
    {
        still,
        sideToSide,
        spiral,
    }

    bossPhases m_currentPhase;

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
        if (m_attackCoroutine == null && m_isInPosition)
        {
            m_attackCoroutine = StartCoroutine(BossAttack(500));
        }
        if (m_isInPosition && m_phaseSwitchCoroutine == null)
        {
            m_phaseSwitchCoroutine = StartCoroutine(PhaseSwitchingCoroutine());
        }

        switch (m_currentPhase)
        {
            case bossPhases.still:
                BossRotation();
                m_fireRate = 0.9f;
                break;
            case bossPhases.sideToSide:
                BossRotation();
                BossMovement();
                m_fireRate = 1.2f;
                break;
            case bossPhases.spiral:
                m_fireRate = 0.25f;
                BossMovement();
                PanickAttack();
                if (m_shouldSpiral)
                {
                    //BossSpiral();
                }
                break;
        }
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
            yield return new WaitForSeconds(m_fireRate);
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
    private void BossSpiral()
    {
        if (!m_isReadyToSpiral)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);

            Vector3 _spiralPos = m_startPos;
            float _range = 0.1f;

            transform.position = Vector2.Lerp(transform.position, _spiralPos, (m_Speed / 2) * Time.deltaTime);

            if (Vector2.Distance(transform.position, _spiralPos) < _range)
            {
                m_isReadyToSpiral = true;
                m_startRotation = transform.rotation;
            }
        }
        else
        {
            float _turnSpeed = 200f;
            transform.Rotate(0, 0, _turnSpeed * Time.deltaTime);

            m_moveDirection = transform.up;

            m_rb.AddForce(m_moveDirection.normalized * m_Speed * Time.deltaTime * 1000, ForceMode2D.Force);

            if(transform.rotation == m_startRotation)
            {
                m_isReadyToSpiral = false;
                m_shouldSpiral = false;
                m_choseNewPhase = true;
            }
        }
    }

    private void BossReturnToStart()
    {
        float _range = 0.1f;
        transform.position = Vector2.Lerp(transform.position, m_startPos, (m_Speed / 2) * Time.deltaTime);

        if (Vector2.Distance(transform.position, m_startPos) < _range)
        {
            m_choseNewPhase = true;
        }
    }

    IEnumerator PhaseSwitchingCoroutine()
    {
        while (true)
        {
            m_currentPhase = (bossPhases)Random.Range(0, System.Enum.GetValues(typeof(bossPhases)).Length);
            yield return new WaitForSeconds(Random.Range(2, 10));

            if (m_choseNewPhase)
            {
                m_currentPhase = (bossPhases)Random.Range(0, System.Enum.GetValues(typeof(bossPhases)).Length);
                m_choseNewPhase = false;
                m_shouldSpiral = true;
            }
            else if (m_currentPhase != bossPhases.spiral)
            {
                yield return new WaitForSeconds(Random.Range(2, 10));
                m_choseNewPhase = true;
            }
        }
    }

    private void PanickAttack()
    {
        float spinSpeed = 1000f;
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
        m_fireRate = 0.1f;
    }

}
