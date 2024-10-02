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

    [Header("Screen Wrap")]
    

    [Header("Other")]
    bool m_IsPaused = false;

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

        m_rb.AddForce(_data.normalized * m_Speed * 10f, ForceMode2D.Force);
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
        Vector3 _screenPos = Camera.main.ScreenToWorldPoint(transform.position);

        float _rightSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x + 0.5f;
        float _leftSideOfScreen = -_rightSideOfScreen - 0.5f;

        if(transform.position.x > _rightSideOfScreen)
        {
            transform.position = new Vector2(_leftSideOfScreen, 0);
        }
        if(transform.position.x < _leftSideOfScreen)
        {
            transform.position = new Vector2(_rightSideOfScreen, 0);
        }
    }
}
