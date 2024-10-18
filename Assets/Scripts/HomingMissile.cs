using UnityEngine;
using UnityEngine.UIElements;

public class HomingMissile : MonoBehaviour
{
    public Vector3 m_moveToPos;
    [SerializeField] float m_speed;
    [SerializeField] float m_range;
    [SerializeField] float m_radius = 1;
    [SerializeField] GameObject m_explotion;
    public GameObject m_crossHair;

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        float _distance = Vector2.Distance(transform.position, m_moveToPos);

        float _lerpFactor = Mathf.Clamp01(Time.deltaTime * m_speed / _distance);

        transform.position = Vector2.Lerp(transform.position, m_moveToPos, _lerpFactor);

        Vector3 _relativePos = m_moveToPos - transform.position;
        Quaternion _lookAt = Quaternion.LookRotation(-transform.forward, _relativePos);

        transform.rotation = _lookAt;

        if (_distance <= m_range)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Destroy(m_crossHair);
        GameObject _explotion = Instantiate(m_explotion, transform.position, Quaternion.identity);
        _explotion.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        Vector2 _position = transform.position;

        Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(_position, m_radius);

        foreach (Collider2D _hitCollider in _hitColliders)
        {
            HitPoints _hitPoints = _hitCollider.gameObject.GetComponent<HitPoints>();
            if( _hitPoints != null)
            {
                _hitPoints.m_HP -= 2;
            }
            EnemyBase _enemyBase = _hitCollider.gameObject.GetComponent<EnemyBase>();
            if(_enemyBase != null)
            {
                _enemyBase.DamageTick();
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(m_crossHair);
            Destroy(collision.gameObject);
            Explode();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}
