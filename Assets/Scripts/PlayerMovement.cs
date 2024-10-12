using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private bool m_zAttack = false;
    [SerializeField] GameObject m_attackParticles;

    [Header("Other")]
    public bool m_IsPaused = false;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
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
    }

    // All the playercontrol functions
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

        if(transform.position.x > _rightSideOfScreen)
        {
            transform.position = new Vector2(_leftSideOfScreen, transform.position.y);
        }
        if(transform.position.x < _leftSideOfScreen)
        {
            transform.position = new Vector2(_rightSideOfScreen, transform.position.y);
        }
    }

    public void PowerUp(string _powerUpName)
    {
        if(_powerUpName == "HP")
        {
            PlayerSettings.Instance.playerHP++;
        }
    }
}
