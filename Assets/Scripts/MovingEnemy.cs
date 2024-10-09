using UnityEngine;

public class MovingEnemy : EnemyBase
{
    private bool m_isAtBotton = false;
    private Vector2 m_moveDirection;

    protected override void Start()
    {
        base.Start();
        MovingEnemyStart();
    }


    protected override void Update()
    {
        base.Update();
        SideToSideMovement();
    }

    private void MovingEnemyStart()
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
        if(m_isInPosition)
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
            
            if(m_enemyType == 1 && transform.position.y >= -3)
            {
                m_rb.AddForce(Vector2.down * m_Speed * Time.deltaTime * 100, ForceMode2D.Force);

                if (m_moveDirection == Vector2.right)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 250);
                }
                else if (m_moveDirection == Vector2.left)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -250);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else
            {
                m_isAtBotton = true;
            }

           if(m_enemyType == 0 || m_isAtBotton)
            {
                if (m_moveDirection == Vector2.right)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (m_moveDirection == Vector2.left)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
        }
    }

}
