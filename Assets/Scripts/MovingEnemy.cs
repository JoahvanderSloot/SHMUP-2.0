using UnityEngine;

public class MovingEnemy : EnemyBase
{
    private Vector2 m_moveDirection;
    private float m_Speed = 8;
    Rigidbody2D m_rb;

    private float m_maxX = 8;
    private float m_maxY = 4;
    private Vector2 m_randomPosition;
    private bool m_isInPosition = false;
    private float m_range = 1f;

    protected override void Start()
    {
        base.Start();

        m_randomPosition = GetRandomPosition();
        MovingEnemtStart();
    }

    Vector2 GetRandomPosition()
    {
        float _randomX = Random.Range(-m_maxX, m_maxX);
        float _randomY = Random.Range(-m_maxY / 4, m_maxY);
        return new Vector2(_randomX, _randomY);
    }

    protected override void Update()
    {
        base.Update();
        SideToSideMovement();

        Debug.Log(m_isInPosition);
    }

    private void MovingEnemtStart()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.drag = 20;
        m_Speed += PlayerSettings.Instance.wave;

        if (Random.Range(0, 2) == 0)
        {
            m_moveDirection = Vector2.left;
        }
        else
        {
            m_moveDirection = Vector2.right;
        }
    }

    private void SideToSideMovement()
    {
        if (!m_isInPosition)
        {
            float distance = Vector2.Distance(transform.position, m_randomPosition);
            float lerpFactor = Mathf.Clamp01(Time.deltaTime * m_Speed / distance);

            transform.position = Vector2.Lerp(transform.position, m_randomPosition, lerpFactor);

            m_isInPosition = distance <= m_range;
        }
        else
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

}
