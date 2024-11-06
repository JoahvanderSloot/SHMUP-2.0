using UnityEngine;

public class HitPoints : MonoBehaviour
{
    public int m_HP;

    private void Update()
    {
        if(tag == "Bullet" && m_HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
