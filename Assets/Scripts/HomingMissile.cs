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
        Destroy(gameObject, 8);
    }

    private void Update()
    {
        // Lerp the missile towards the crossHair which gets placed in the GameManager when you shoot
        m_moveToPos = m_crossHair.transform.position;

        float _distance = Vector2.Distance(transform.position, m_moveToPos);

        float _lerpFactor = Mathf.Clamp01(Time.deltaTime * m_speed / _distance);

        transform.position = Vector2.Lerp(transform.position, m_moveToPos, _lerpFactor);

        Vector3 _relativePos = m_moveToPos - transform.position;
        Quaternion _lookAt = Quaternion.LookRotation(-transform.forward, _relativePos);

        transform.rotation = _lookAt;

        // Explode when the missile is at its target
        if (_distance <= m_range)
        {
            Explode();
        }
    }

    private void Explode()
    {
        // Stop the launchin sound from playing
        FindObjectOfType<AudioManager>().Stop("Missile");

        //Remove the crosshair and instantiate the explotion (which only particles and doesnt deal damage)
        Destroy(m_crossHair);
        GameObject _explotion = Instantiate(m_explotion, transform.position, Quaternion.identity);
        _explotion.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        Vector2 _position = transform.position;

        Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(_position, m_radius);

        // Deal damage to any object which is in the range of the splash damage from the explotion
        // And run the damagetick function when one of these object in the explotion radius is a enemy
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
        //On direct impact (so not in the explotion range) it imediatly kills any normaly enemy and deals 3 HitPoints of damage to the boss
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(m_crossHair);
            Boss _boss = collision.gameObject.GetComponent<Boss>();
            if (_boss == null)
            {
                HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
                if(_hitPoints != null)
                {
                    _hitPoints.m_HP = 0;
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
            else
            {
                _boss.m_isHit = true;
                HitPoints _hitPoints = collision.gameObject.GetComponent<HitPoints>();
                _hitPoints.m_HP -= 3;
            }
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
