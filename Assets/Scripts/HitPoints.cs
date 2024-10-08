using UnityEngine;

public class HitPoints : MonoBehaviour
{
    public int m_HP;

    private void Update()
    {
        if(m_HP <= 0)
        {
            if(tag != "Enemy")
            {
                Destroy(gameObject);
            }
        }
    }
}
