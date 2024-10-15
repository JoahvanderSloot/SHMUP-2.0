using System.Collections;
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
    [SerializeField] List<GameObject> m_enemyPrefabs;
    [SerializeField] GameObject m_boss;
    [SerializeField] Vector3 m_enemySpawnPos;

    [Header("Wave Settings")]
    [SerializeField] int m_minEnemiesPerWave = 3;
    [SerializeField] int m_maxEnemiesPerWave = 6;
    [SerializeField] float m_enemySpawnDelay = 1f;

    List<GameObject> m_currentWaveEnemies = new List<GameObject>();
    int m_waveCount = 0;
    bool m_isSpawningWave = false;
    Coroutine m_enemySpawner;

    [Header("Shit Repair")]
    public bool m_canRepair = true;
    [SerializeField] GameObject m_repairObject;

    private void Start()
    {
        m_movement = m_player.GetComponent<PlayerMovement>();
        m_playerSpriteRenderer = m_player.GetComponent<SpriteRenderer>();
        m_repairObject.SetActive(false);
        SpawnNewWave();
        m_canRepair = true;
    }

    private void Update()
    {
        if (m_waveCount > 0)
        {
            PlayerSettings.Instance.wave = m_waveCount;
        }

        if (PlayerSettings.Instance.playerHP <= 0)
        {
            if (m_canRepair && !PlayerSettings.Instance.isRepairing)
            {
                m_repairObject.SetActive(true);
                PlayerSettings.Instance.isRepairing = true;
                m_canRepair = false;
            }
            else if (!PlayerSettings.Instance.isRepairing)
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        if (!PlayerSettings.Instance.isRepairing)
        {
            m_repairObject.SetActive(false);
        }

        if (!m_movement.m_IsPaused)
        {
            CheckWaveStatus();
        }

        if (PlayerSettings.Instance.shipLevel < m_playerSprites.Count)
        {
            m_playerSpriteRenderer.sprite = m_playerSprites[PlayerSettings.Instance.shipLevel];
        }

        if (PlayerSettings.Instance.shipLevel >= 2)
        {
            PlayerSettings.Instance.shipLevel = 2;
        }

        if (PlayerSettings.Instance.playerHP > 3)
        {
            PlayerSettings.Instance.playerHP = 3;
        }
    }

    private void SpawnNewWave()
    {
        if (m_isSpawningWave) return;
        m_isSpawningWave = true;

        m_currentWaveEnemies.Clear();

        if (m_waveCount > 0 && m_waveCount % 5 == 0)
        {
            SpawnBoss();
        }
        else
        {
            m_enemySpawner = StartCoroutine(SpawnEnemiesWithDelay());
        }

        m_waveCount++;
    }

    private void SpawnBoss()
    {
        GameObject boss = Instantiate(m_boss, m_enemySpawnPos, Quaternion.identity);
        m_currentWaveEnemies.Add(boss);
        m_isSpawningWave = false;
    }

    private void CheckWaveStatus()
    {

        m_currentWaveEnemies.RemoveAll(enemy => enemy == null);

        if (m_currentWaveEnemies.Count == 0 && !m_isSpawningWave)
        {
            SpawnNewWave();
        }
    }

    IEnumerator SpawnEnemiesWithDelay()
    {
        int _enemyCount = Random.Range(m_minEnemiesPerWave, m_maxEnemiesPerWave);

        for (int i = 0; i < _enemyCount; i++)
        {
            int randomEnemyIndex = Random.Range(0, m_enemyPrefabs.Count);
            GameObject _enemy = Instantiate(m_enemyPrefabs[randomEnemyIndex], m_enemySpawnPos, Quaternion.identity);
            EnemyBase _enemyBase = _enemy.GetComponent<EnemyBase>();
            _enemyBase.m_enemyType = Random.Range(0, 2);
            m_currentWaveEnemies.Add(_enemy);

            yield return new WaitForSeconds(m_enemySpawnDelay);
        }

        m_isSpawningWave = false;
    }
}
