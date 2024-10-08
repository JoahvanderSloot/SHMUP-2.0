using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] HitPoints m_hitPoints;
    [SerializeField] GameObject m_powerUp;

    protected virtual void Start()
    {
        m_hitPoints = GetComponent<HitPoints>();
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
    }
}
