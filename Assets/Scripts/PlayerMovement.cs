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
    Coroutine m_newCoroutine;

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


        if (_data == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -11);
        }
        else if (_data == Vector2.left)
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
            Instantiate(m_Bullet, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(m_ShootTimer / 2);
        }  
    }

    public void Pause()
    {
        m_IsPaused = !m_IsPaused;
    }

    // Other player logic
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
