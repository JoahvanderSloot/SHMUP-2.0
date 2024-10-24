using System.Collections;
using UnityEngine;

public class Boss : EnemyBase
{
    [SerializeField] GameObject m_bullet;
    Vector2 m_moveDirection;
    [SerializeField] float m_fireRate;

    Coroutine m_attackCoroutine;
    Coroutine m_phaseSwitchCoroutine;

    private enum bossPhases
    {
        still,
        sideToSide,
        panic
    }

    bossPhases m_currentPhase;

    protected override void Start()
    {
        base.Start();

        m_enemyType = 0;

        // Set the HP of the boss according to wich wave you are on
        m_hitPoints.m_HP = (4 * GameInfoSingleton.Instance.playerSettings.wave) + 12;

        m_moveDirection = new Vector2(Random.value > 0.5f ? 1 : -1, 0);
        m_Speed += (m_floatWave / 4);       
    }

    protected override void Update()
    {
        base.Update();

        // Start the coroutines for shooting and movement
        if (m_attackCoroutine == null && m_isInPosition)
        {
            m_attackCoroutine = StartCoroutine(BossAttack(500));
        }
        if (m_isInPosition && m_phaseSwitchCoroutine == null)
        {
            m_phaseSwitchCoroutine = StartCoroutine(PhaseSwitchingCoroutine());
        }

        // Call the correct function and set the fire rate acording the the attackphase of the boss
        switch (m_currentPhase)
        {
            case bossPhases.still:
                BossRotation();
                m_fireRate = 0.5f;
                break;
            case bossPhases.sideToSide:
                BossRotation();
                BossMovement();
                m_fireRate = 0.9f;
                break;
            case bossPhases.panic:
                BossMovement();
                PanicAttack();
                break;
        }
    }

    // BossAttack makes the boss shoot
    IEnumerator BossAttack(float _shootForce)
    {
        while (true)
        {
            yield return new WaitForSeconds(m_fireRate);
            GameObject _instantedBullet = Instantiate(m_bullet, transform.position, transform.rotation);
            BulletScript _enemyBullet = _instantedBullet.GetComponent<BulletScript>();
            _enemyBullet.m_shootForce = _shootForce;
            _enemyBullet.m_ShootDirection = transform.up;
        }
    }

    // BossRotation makes the boss look at the player so when it is standing still or moving side to side it still shoots towards the player
    private void BossRotation()
    {
        GameObject _player = GameObject.FindWithTag("Player");
        Vector3 _relativePos = _player.transform.position;
        Quaternion _lookAt = Quaternion.LookRotation(Vector3.forward, _relativePos - transform.position);
        transform.rotation = _lookAt;
    }

    // BossMovement makes the boss move side to side
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

    // This coroutine swicthes the bossphase, the time it is in a phase is shorter on the still phase (where it stands still) because otherwise it can get boring
    IEnumerator PhaseSwitchingCoroutine()
    {
        while (true)
        {
            m_currentPhase = (bossPhases)Random.Range(0, System.Enum.GetValues(typeof(bossPhases)).Length);
            if(m_currentPhase != bossPhases.still)
            {
                yield return new WaitForSeconds(Random.Range(2, 10));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(2, 5));
            }
            
        }
    }

    // PanicAttack makes the boss spin around and shoot really fast
    private void PanicAttack()
    {
        float spinSpeed = 1000f;
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
        m_fireRate = 0.1f;
    }

}
