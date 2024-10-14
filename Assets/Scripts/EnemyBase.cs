using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Generic enemy info")]
    [SerializeField] GameObject m_powerUp;
    public int m_enemyType = 1;
    public bool m_isHit = false;
    private float m_hitTimer = 0;
    private SpriteRenderer m_spriteRenderer;

    [Header("Protected variables")]
    [SerializeField] protected HitPoints m_hitPoints;
    [SerializeField] protected int m_Score;
    protected bool m_isInPosition = false;
    protected float m_Speed = 8;
    protected Rigidbody2D m_rb;
    protected float m_floatWave;

    [Header("Lerp into position variables")]
    private float m_maxX = 8;
    private float m_maxY = 4;
    private Vector3 m_randomPosition;
    private float m_range = 0.5f;

    protected virtual void Start()
    {
        m_hitPoints = GetComponent<HitPoints>();
        m_randomPosition = GetRandomPosition();

        m_rb = GetComponent<Rigidbody2D>();
        m_rb.drag = 20;

        m_floatWave += PlayerSettings.Instance.wave;

        if (m_enemyType == 1)
        {
            m_Score *= 2;
        }

        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    Vector3 GetRandomPosition()
    {
        float _randomX = Random.Range(-m_maxX, m_maxX);
        float _randomY = Random.Range(1, m_maxY);
        return new Vector3(_randomX, _randomY, 180);
    }

    protected virtual void Update()
    {
        if(m_hitPoints.m_HP <= 0)
        {
            if(Random.Range(0, 4) == 1)
            {
                Debug.Log("Spawn powerup");
                Instantiate(m_powerUp, transform.position, Quaternion.identity);
                PlayerSettings.Instance.score += m_Score * PlayerSettings.Instance.wave;
                Destroy(gameObject);
            }
            else
            {
                PlayerSettings.Instance.score += m_Score * PlayerSettings.Instance.wave;
                Destroy(gameObject);
            }
        }

        if (!m_isInPosition)
        {
            float _distance = Vector2.Distance(transform.position, m_randomPosition);

            float _lerpFactor = Mathf.Clamp01(Time.deltaTime * m_Speed / _distance);

            transform.position = Vector2.Lerp(transform.position, m_randomPosition, _lerpFactor);

            Vector3 _relativePos = m_randomPosition - transform.position;
            Quaternion _lookAt = Quaternion.LookRotation(-transform.forward, _relativePos);
            transform.rotation = _lookAt;

            if (_distance <= m_range)
            {
                m_isInPosition = true;
            }
        }

        if (m_isHit)
        {
            m_hitTimer += Time.deltaTime * 10;
            if(m_hitTimer >= 1)
            {
                Color _tickColor = m_spriteRenderer.color;
                _tickColor.a = 1f;
                m_spriteRenderer.color = _tickColor;
                m_isHit = false;
                m_hitTimer = 0;
            }
        }
    }

    public void DamageTick()
    {
        Color _tickColor = m_spriteRenderer.color;
        _tickColor.a = 50f / 255f;
        m_spriteRenderer.color = _tickColor;

        m_isHit = true;
    }
}
