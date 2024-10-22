using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerMovement;

public class UImanager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] TextMeshProUGUI m_score;
    PlayerMovement m_playerMovement;
    [SerializeField] TextMeshProUGUI m_wave;
    [Header("Menu")]
    [SerializeField] GameObject m_escMenu;
    [SerializeField] List<GameObject> m_allUIobjects;
    [Header("Damage")]
    [SerializeField] List<Image> m_lives;
    [SerializeField] Image m_damageFlash;
    float m_damageAlpha;
    int m_previousHP;
    [Header("Special Attacks")]
    [SerializeField] GameObject m_zAttackUI;
    [SerializeField] TextMeshProUGUI m_missileCount;
    [SerializeField] Slider m_wormHoleCooldown;

    private void Start()
    {
        m_playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        m_previousHP = GameInfoSingleton.Instance.playerSettings.playerHP;
    }

    private void Update()
    {
        m_score.text = GameInfoSingleton.Instance.playerSettings.IGN + ": " + GameInfoSingleton.Instance.playerSettings.score.ToString();
        m_wave.text = "Wave: " + GameInfoSingleton.Instance.playerSettings.wave.ToString();
        m_missileCount.text = GameInfoSingleton.Instance.playerSettings.missileCount.ToString();

        if (m_playerMovement.m_IsPaused)
        {
            m_escMenu.SetActive(true);
        }
        else
        {
            m_escMenu.SetActive(false);
        }

        if (m_playerMovement.m_IsPaused)
        {
            m_escMenu.transform.localScale = Vector2.Lerp(m_escMenu.transform.localScale, Vector2.one, 10 * Time.unscaledDeltaTime);
        }
        else
        {
            m_escMenu.transform.localScale = Vector2.Lerp(m_escMenu.transform.localScale, Vector2.zero, 10 * Time.unscaledDeltaTime);
        }

        if (!m_playerMovement.m_zAttack)
        {
            m_zAttackUI.SetActive(true);
        }
        else
        {
            m_zAttackUI.SetActive(false);
        }

        if (GameInfoSingleton.Instance.playerSettings.isRepairing)
        {
            foreach (GameObject _obj in m_allUIobjects)
            {
                _obj.SetActive(false);
                m_zAttackUI.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject _obj in m_allUIobjects)
            {
                _obj.SetActive(true);
            }
        }

        if (m_playerMovement.m_currentWormHoleState == wormHoleState.notInArsenal)
        {
            m_wormHoleCooldown.value = 10 - m_playerMovement.m_wormHoleTimer;
        }
        else
        {
            m_wormHoleCooldown.value = 10;
        }
       

        UpdateLives();
        Damage();
    }

    private void UpdateLives()
    {
        for (int i = 0; i < m_lives.Count; i++)
        {
            Color _currentColor = m_lives[i].color;

            if (i < GameInfoSingleton.Instance.playerSettings.playerHP)
            {
                _currentColor.a = 1f;
            }
            else
            {
                _currentColor.a = 50f / 255f;
            }

            m_lives[i].color = _currentColor;
        }
    }

    private void Damage()
    {
        Color _flashColor = m_damageFlash.color;
        if(GameInfoSingleton.Instance.playerSettings.playerHP < m_previousHP)
        {
            _flashColor.a = m_damageAlpha / 250;
            m_damageAlpha -= Time.deltaTime * 300;
            if (m_damageAlpha <= 0)
            {
                m_previousHP = GameInfoSingleton.Instance.playerSettings.playerHP;
                m_damageAlpha = 100;  
            }

            m_damageFlash.color = _flashColor;
        }
        else
        {
            _flashColor.a = m_damageAlpha = 100;
            Color _normalColor = m_damageFlash.color;
            _normalColor.a = 0f;
            m_damageFlash.color = _normalColor;
        }
       
        if(GameInfoSingleton.Instance.playerSettings.playerHP > m_previousHP)
        {
            m_previousHP = GameInfoSingleton.Instance.playerSettings.playerHP;
        }
    }
}
