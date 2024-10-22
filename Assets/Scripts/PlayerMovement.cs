using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] InputActionReference m_moveInput;
    [SerializeField] float m_speed;
    [SerializeField] float m_drag;
    Rigidbody2D m_rb;

    [Header("Shooting")]
    [SerializeField] float m_shootTimer;
    [SerializeField] GameObject m_bullet;
    [SerializeField] float m_playerShootForce;
    Coroutine m_newCoroutine;
    public bool m_zAttack = false;
    [SerializeField] GameObject m_attackParticles;

    [Header("Powerups")]
    public bool m_shield = false;
    float m_shieldTimer = 0;
    [SerializeField] float m_shieldDuration = 50;
    [SerializeField] float m_shieldRadius = 2f;
    [SerializeField] GameObject m_shieldObject;
    Coroutine m_shieldCoroutine;

    [Header("Other")]
    public bool m_IsPaused = false;
    public bool m_healingPickedUp = false;
    private float m_flashGreenTimer = 0;
    private SpriteRenderer m_spriteRenderer;

    [Header("WormHole")]
    [SerializeField] GameObject m_wormHolePref;
    GameObject m_wormHoleObj;
    public float m_wormHoleTimer;
    [SerializeField] GameObject m_particles;
    [SerializeField] InputActionReference m_cursorInput;

    public enum wormHoleState
    {
        notInArsenal,
        inArsenal,
        inScene
    }

    public wormHoleState m_currentWormHoleState;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_currentWormHoleState = wormHoleState.inArsenal;
    }

    private void Update()
    {
        if (!m_IsPaused)
        {
            Move();
            ScreenWrap();
            WormHoleLogic();
        }

        //MoveCursorWithGamepad();

        m_rb.drag = m_drag;

        if (m_IsPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

        m_shieldObject.SetActive(m_shield);

        if (m_shield)
        {
            if(m_shieldCoroutine == null)
            {
                m_shieldCoroutine = StartCoroutine(ShieldFlicker());
            }

            m_shieldTimer += Time.deltaTime;
            if (m_shieldTimer >= m_shieldDuration)
            {
                m_shield = false;
                m_shieldTimer = 0;
                StopCoroutine(m_shieldCoroutine);
                m_shieldCoroutine = null;
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_shieldRadius);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Destroy(collider.gameObject);
                        Instantiate(GameInfoSingleton.Instance.playerSettings.explotion, collider.gameObject.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        GameInfoSingleton.Instance.playerSettings.shieldIsActive = m_shield;

        if (m_healingPickedUp)
        {
            m_flashGreenTimer += Time.deltaTime * 10;
            if (m_flashGreenTimer >= 2)
            {
                Color _tickColor = m_spriteRenderer.color;
                _tickColor.r = 1f;
                m_spriteRenderer.color = _tickColor;
                m_healingPickedUp = false;
                m_flashGreenTimer = 0;
            }
        }
    }

    public void Move()
    {
        Vector2 _data = m_moveInput.action.ReadValue<Vector2>();

        m_rb.AddForce(_data.normalized * m_speed * Time.deltaTime * 1000, ForceMode2D.Force);

        if (_data == Vector2.right && !m_IsPaused)
        {
            transform.rotation = Quaternion.Euler(0, 0, -11);
        }
        else if (_data == Vector2.left && !m_IsPaused)
        {
            transform.rotation = Quaternion.Euler(0, 0, 11);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void Fire(CallbackContext _context)
    {
        if (_context.performed)
        {
            m_newCoroutine = StartCoroutine(ShootLogic());
        }
        if (_context.canceled)
        {
            StopCoroutine(m_newCoroutine);
        }
    }

    IEnumerator ShootLogic()
    {
        while (true)
        {
            GameObject _shotBullet = Instantiate(m_bullet, transform.position, Quaternion.identity);
            _shotBullet.GetComponent<SpriteRenderer>().color = Color.white;
            BulletScript _bulletScript = _shotBullet.GetComponent<BulletScript>();
            _bulletScript.m_canDamageEnemy = true;
            _bulletScript.m_damage = GameInfoSingleton.Instance.playerSettings.shipLevel + 1;
            _bulletScript.m_ShootDirection = Vector2.up;
            _bulletScript.m_shootForce = m_playerShootForce;
            yield return new WaitForSeconds(m_shootTimer / 2);
        }
    }

    public void Pause()
    {
        if (!GameInfoSingleton.Instance.playerSettings.isRepairing)
        {
            m_IsPaused = !m_IsPaused;
        }
        else
        {
            m_IsPaused = true;
        }
    }

    public void InstaKill()
    {
        if (!m_zAttack && !m_IsPaused)
        {
            List<GameObject> _targets = new List<GameObject>();

            _targets.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            _targets.AddRange(GameObject.FindGameObjectsWithTag("Bullet"));

            Instantiate(m_attackParticles, transform.position, Quaternion.identity);

            foreach (GameObject _target in _targets)
            {
                if (_target.GetComponent<Boss>() == null)
                {
                    if (_target.CompareTag("Enemy"))
                    {
                        EnemyBase _enemyBase = _target.GetComponent<EnemyBase>();
                        GameInfoSingleton.Instance.playerSettings.score += _enemyBase.m_Score;
                        Instantiate(GameInfoSingleton.Instance.playerSettings.explotion, _target.transform.position, Quaternion.identity);
                    }
                    Destroy(_target);
                }
            }

            m_zAttack = true;
        }
    }

    public void ActivateMissile()
    {
        GameManager _gameManager = FindAnyObjectByType<GameManager>();
        if (_gameManager != null && !m_IsPaused)
        {
            _gameManager.m_missileAttack = !_gameManager.m_missileAttack;
        }
        
    }

    public void WormHole(CallbackContext _context)
    {
        Vector2 _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!m_IsPaused && _context.performed)
        {
            if (m_currentWormHoleState == wormHoleState.inArsenal)
            {
                m_wormHoleObj = Instantiate(m_wormHolePref, _mouseWorldPos, Quaternion.identity);
                m_currentWormHoleState = wormHoleState.inScene;
            }
            else if (m_currentWormHoleState == wormHoleState.inScene)
            {
                if (m_wormHoleObj != null)
                {
                    transform.position = m_wormHoleObj.transform.position;
                    Destroy(m_wormHoleObj);
                    m_currentWormHoleState = wormHoleState.notInArsenal;
                    m_wormHoleTimer = 10f;
                }
            }
        }
    }

    private void WormHoleLogic()
    {
        if (m_currentWormHoleState == wormHoleState.inScene)
        {
            Vector2 _mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_wormHoleObj.transform.position = _mouseWorldPos;
        }

        if (m_currentWormHoleState == wormHoleState.notInArsenal)
        {
            m_wormHoleTimer -= Time.deltaTime * 2.5f;
            if (m_wormHoleTimer <= 0)
            {
                m_wormHoleTimer = 0;
                m_currentWormHoleState = wormHoleState.inArsenal;
            }
        }

        if (transform.position.y != -3)
        {
            Vector3 _lerpPos = new Vector3(transform.position.x, -3, transform.position.z);
            float _distance = Vector2.Distance(transform.position, _lerpPos);
            float _lerpFactor = Mathf.Clamp01(Time.deltaTime * m_speed / _distance);

            transform.position = Vector2.Lerp(transform.position, _lerpPos, _lerpFactor);

            m_particles.SetActive(false);
        }
        else
        {
            m_particles.SetActive(true);
        }
    }

    public void MoveCursorWithGamepad()
    {
        Vector2 gamepadInput = m_cursorInput.action.ReadValue<Vector2>();

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float scaleFactor = 0.1f;
        Vector3 newCursorPosition = new Vector3(
            mousePosition.x + gamepadInput.x * scaleFactor,
            mousePosition.y + gamepadInput.y * scaleFactor,
            mousePosition.z
        );

        Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(newCursorPosition));
    }

    private void ScreenWrap()
    {
        float _rightSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x + 0.15f;
        float _leftSideOfScreen = -_rightSideOfScreen - 0.15f;

        if (transform.position.x > _rightSideOfScreen)
        {
            transform.position = new Vector2(_leftSideOfScreen, transform.position.y);
        }
        if (transform.position.x < _leftSideOfScreen)
        {
            transform.position = new Vector2(_rightSideOfScreen, transform.position.y);
        }
    }

    public void PowerUp(string _powerUpName)
    {
        if (_powerUpName == "HP")
        {
            if(GameInfoSingleton.Instance.playerSettings.playerHP < 3)
            {
                GameInfoSingleton.Instance.playerSettings.playerHP++;
                FlashGreen();
            }
            else
            {
                 GameInfoSingleton.Instance.playerSettings.score += 25;
            }
        }
        if (_powerUpName == "Upgrade")
        {
            GameInfoSingleton.Instance.playerSettings.shipLevel++;
        }
        if (_powerUpName == "Instakill")
        {
            if (m_zAttack == true)
            {
                m_zAttack = false;
            }
            else
            {
                GameInfoSingleton.Instance.playerSettings.score += 25;
            }
        }
        if (_powerUpName == "Shield")
        {
            if (!m_shield)
            {
                m_shield = true;
            }
            else
            {
                m_shieldTimer = 0;
            }
        }
        if(_powerUpName == "Missile")
        {
            GameInfoSingleton.Instance.playerSettings.missileCount += 3;
        }
    }

    private void FlashGreen()
    {
        Color _tickColor = m_spriteRenderer.color;
        _tickColor.r = 10f / 255f;
        m_spriteRenderer.color = _tickColor;

        m_healingPickedUp = true;
    }

    IEnumerator ShieldFlicker()
    {
        SpriteRenderer _shieldSR = m_shieldObject.GetComponent<SpriteRenderer>();

        Color _shieldColor = _shieldSR.color;

        _shieldColor.a = 25f / 255f;
        _shieldSR.color = _shieldColor;

        while (true)
        {
            if (m_shieldTimer >= m_shieldDuration - m_shieldDuration / 7)
            {
                while (m_shieldTimer >= m_shieldDuration - m_shieldDuration / 7)
                {
                    _shieldColor.a = 5f / 255f;
                    _shieldSR.color = _shieldColor;
                    yield return new WaitForSeconds(0.1f);

                    _shieldColor.a = 25f / 255f;
                    _shieldSR.color = _shieldColor;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_shieldRadius);
    }
}
