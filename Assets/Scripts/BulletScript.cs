using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D m_rb;
    [SerializeField] float m_shootForce;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();

        m_rb.AddForce(Vector2.up * m_shootForce, ForceMode2D.Force);

        Destroy(gameObject, 5);
    }
}
