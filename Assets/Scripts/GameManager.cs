using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject m_Player;
    PlayerMovement m_Movement;

    [Header("Enemys")]
    [SerializeField] int m_Wave = 0;

    private void Start()
    {
        m_Movement = m_Player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(PlayerSettings.Instance.playerHP <= 0)
        {
            Destroy(m_Player);
            SceneManager.LoadScene("GameOver");
        }

        if (!m_Movement.m_IsPaused)
        {
            EnemyWaves();
        }
    }

    private void EnemyWaves()
    {

    }
}
