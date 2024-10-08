using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] int m_DamagePoints;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerSettings.Instance.playerHP -= m_DamagePoints;
            //instantiate explotion
            //playe sound
            Destroy(gameObject);
        }
    }
}
