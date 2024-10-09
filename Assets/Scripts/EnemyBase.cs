using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Generic enemy info")]
    [SerializeField] HitPoints m_hitPoints;
    [SerializeField] GameObject m_powerUp;
    public int m_enemyType = 1;

    [Header("Protected variables")]
    protected bool m_isInPosition = false;
    protected float m_Speed = 8;
    protected Rigidbody2D m_rb;

    [Header("Lerp into position variables")]
    private float m_maxX = 8;
    private float m_maxY = 4;
    private Vector3 m_randomPosition;
    private float m_range = 0.5f;

    protected virtual void Start()
    {
        m_hitPoints = GetComponent<HitPoints>();
        m_randomPosition = GetRandomPosition();
    }

    Vector3 GetRandomPosition()
    {
        float _randomX = Random.Range(-m_maxX, m_maxX);
        float _randomY = Random.Range(-m_maxY / 4, m_maxY);
        return new Vector3(_randomX, _randomY, 180);
    }

    protected virtual void Update()
    {
        if(m_hitPoints.m_HP <= 0)
        {
            if(Random.Range(0, 4) == 1)
            {
                Debug.Log("Spawn powerup");
                //Instantiate(m_powerUp);
                PlayerSettings.Instance.score += 5 * PlayerSettings.Instance.wave;
                Destroy(gameObject);
            }
            else
            {
                PlayerSettings.Instance.score += 5 * PlayerSettings.Instance.wave;
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
    }
}
