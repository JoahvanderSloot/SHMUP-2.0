using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject m_player;
    PlayerMovement m_movement;
    [SerializeField] List<Sprite> m_playerSprites;
    SpriteRenderer m_playerSpriteRenderer;

    [Header("Enemies")]
    [SerializeField] List<GameObject> m_enemys;

    private void Start()
    {
        m_movement = m_player.GetComponent<PlayerMovement>();
        m_playerSpriteRenderer = m_player.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (PlayerSettings.Instance.playerHP <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }

        if (!m_movement.m_IsPaused)
        {
            EnemyWaves();
        }

        if (PlayerSettings.Instance.shipLevel < m_playerSprites.Count)
        {
            m_playerSpriteRenderer.sprite = m_playerSprites[PlayerSettings.Instance.shipLevel];
        }

        if(PlayerSettings.Instance.shipLevel >= 2)
        {
            PlayerSettings.Instance.shipLevel = 2;
        }
        if(PlayerSettings.Instance.playerHP > 3)
        {
            PlayerSettings.Instance.playerHP = 3;
        }
    }

    private void EnemyWaves()
    {
        
    }
}
