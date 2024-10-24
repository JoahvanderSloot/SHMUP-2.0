using UnityEngine;

public class MovingEnemy : EnemyBase
{
    private bool m_isAtBottom = false;
    private Vector2 m_moveDirection;
    private bool m_moveDown = true;
    private float m_bottomTimer = 0;
    private float m_waitTimeAtBottom = 4.0f;

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
        m_Speed += (m_floatWave / 4);
        m_moveDirection = new Vector2(Random.value > 0.5f ? 1 : -1, 0);
    }

    private void SideToSideMovement()
    {
        // If the moving enemy is in the startposition it starts moving left and right
        if (m_isInPosition)
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

            // When the enemy type is 1 it moves side to side but also moves down
            if (m_enemyType == 1 && m_moveDown)
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

                // When the enemy is at the bottom it sets that bool to true and stops it from moving down any further
                if (transform.position.y <= -3)
                {
                    m_isAtBottom = true;
                    m_moveDown = false;
                    m_bottomTimer = 0;
                }
            }

            // When the enemy is at the button it moves side to side for a while down there, and when its done it starts flying up again
            if (m_isAtBottom)
            {
                m_bottomTimer += Time.deltaTime;

                if (m_bottomTimer >= m_waitTimeAtBottom)
                {
                    m_rb.AddForce(Vector2.up * m_Speed * Time.deltaTime * 100, ForceMode2D.Force);

                    if (m_moveDirection == Vector2.right)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, 290);
                    }
                    else if (m_moveDirection == Vector2.left)
                    {
                        transform.rotation = Quaternion.Euler(0, 0, -290);
                    }

                    if (transform.position.y >= 4)
                    {
                        m_moveDown = true;
                        m_isAtBottom = false;
                    }
                }
            }

            // thiss handles the rotation for if the enemy is at the bottom
            if (m_enemyType == 0 || m_isAtBottom && m_bottomTimer < m_waitTimeAtBottom)
            {
                if (m_moveDirection == Vector2.right)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (m_moveDirection == Vector2.left)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }
        }
    }
}
