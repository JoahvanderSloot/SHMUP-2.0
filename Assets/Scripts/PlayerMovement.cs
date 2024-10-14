using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] InputActionReference m_MoveInput;
    [SerializeField] float m_Speed;
    [SerializeField] float m_Drag;
    Rigidbody2D m_rb;

    [Header("Shooting")]
    [SerializeField] float m_ShootTimer;
    [SerializeField] GameObject m_Bullet;
    [SerializeField] float m_playerShootForce;
    Coroutine m_newCoroutine;
    public bool m_zAttack = false;
    [SerializeField] GameObject m_attackParticles;

    [Header("Powerups")]
    [SerializeField] bool m_shield = false;
    float m_shieldTimer = 0;
    [SerializeField] float m_shieldDuration = 50;
    [SerializeField] float m_shieldRadius = 2f;
    [SerializeField] GameObject m_shieldObject;
    Coroutine m_shieldCoroutine;

    [Header("Other")]
    public bool m_IsPaused = false;
    public bool m_isHit = false;
    private float m_hitTimer = 0;
    private SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!m_IsPaused)
        {
            Move();
            ScreenWrap();
        }

        m_rb.drag = m_Drag;

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
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_shieldRadius);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Destroy(collider.gameObject);
                    }
                }
            }
        }

        if (m_isHit)
        {
            m_hitTimer += Time.deltaTime * 10;
            if (m_hitTimer >= 1)
            {
                Color _tickColor = m_spriteRenderer.color;
                _tickColor.r = 1f;
                m_spriteRenderer.color = _tickColor;
                m_isHit = false;
                m_hitTimer = 0;
            }
        }
    }

    public void Move()
    {
        Vector2 _data = m_MoveInput.action.ReadValue<Vector2>();

        m_rb.AddForce(_data.normalized * m_Speed * Time.deltaTime * 1000, ForceMode2D.Force);

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
            GameObject _shotBullet = Instantiate(m_Bullet, transform.position, Quaternion.identity);
            _shotBullet.GetComponent<SpriteRenderer>().color = Color.white;
            BulletScript _bulletScript = _shotBullet.GetComponent<BulletScript>();
            _bulletScript.m_canDamageEnemy = true;
            _bulletScript.m_damage = PlayerSettings.Instance.shipLevel + 1;
            _bulletScript.m_ShootDirection = Vector2.up;
            _bulletScript.m_shootForce = m_playerShootForce;
            yield return new WaitForSeconds(m_ShootTimer / 2);
        }
    }

    public void Pause()
    {
        m_IsPaused = !m_IsPaused;
    }

    public void InstaKill()
    {
        if (!m_zAttack)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            Instantiate(m_attackParticles, transform.position, Quaternion.identity);

            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
            m_zAttack = true;
        }
    }

    void ScreenWrap()
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
            PlayerSettings.Instance.playerHP++;
            FlashGreen();
        }
        if (_powerUpName == "Upgrade")
        {
            PlayerSettings.Instance.shipLevel++;
        }
        if (_powerUpName == "Instakill")
        {
            m_zAttack = false;
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
    }

    private void FlashGreen()
    {
        Color _tickColor = m_spriteRenderer.color;
        _tickColor.r = 10f / 255f;
        m_spriteRenderer.color = _tickColor;

        m_isHit = true;
    }

    IEnumerator ShieldFlicker()
    {
        SpriteRenderer _shieldSR = m_shieldObject.GetComponent<SpriteRenderer>();

        while (true)
        {
            if (m_shieldTimer >= m_shieldDuration / 8)
            {
                while (m_shieldTimer >= m_shieldDuration / 8)
                {
                    Color _shieldColor = _shieldSR.color;

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
