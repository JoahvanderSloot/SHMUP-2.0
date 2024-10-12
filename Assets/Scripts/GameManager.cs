using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject m_player;
    PlayerMovement m_movement;

    [Header("Enemys")]
    [SerializeField] List<GameObject> m_enemys;

    private void Start()
    {
        m_movement = m_player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(PlayerSettings.Instance.playerHP <= 0)
        {
            Destroy(m_player);
            SceneManager.LoadScene("GameOver");
        }

        if (!m_movement.m_IsPaused)
        {
            EnemyWaves();
        }
    }

    private void EnemyWaves()
    {

    }
}
