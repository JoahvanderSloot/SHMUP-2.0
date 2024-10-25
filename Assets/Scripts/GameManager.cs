using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    [Header("Ship Repair")]
    public bool m_canRepair = true;
    [SerializeField] GameObject m_repairObject;

    [Header("MissileAttack")]
    public bool m_missileAttack = false;
    [SerializeField] GameObject m_missile;
    [SerializeField] GameObject m_crossHairPref;
    Vector3 m_crossHairPos;
    GameObject m_crossHair;

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
        // Sets the wave count in the playerSettings to the correct wave
        if (m_waveCount > 0)
        {
            GameInfoSingleton.Instance.playerSettings.wave = m_waveCount;
        }

        // When the player HP is 0 or lower it checks if you alreay repared your ship this game
        // If you have it loads the GameOver scene and if you havent it starts the repairing task and sets m_canRepair to false
        if (GameInfoSingleton.Instance.playerSettings.playerHP <= 0)
        {
            if (m_canRepair && !GameInfoSingleton.Instance.playerSettings.isRepairing)
            {
                m_repairObject.SetActive(true);
                GameInfoSingleton.Instance.playerSettings.isRepairing = true;
                m_canRepair = false;
            }
            else if (!GameInfoSingleton.Instance.playerSettings.isRepairing)
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        // This sets the repairing task object to false if you stop repairing
        if (!GameInfoSingleton.Instance.playerSettings.isRepairing)
        {
            m_repairObject.SetActive(false);
        }

        // Checks for the status of the wave when you are not paused
        if (!m_movement.m_IsPaused)
        {
            CheckWaveStatus();
        }

        // This gives your player the correct sprite according the the shiplevel (powerups)
        if (GameInfoSingleton.Instance.playerSettings.shipLevel < m_playerSprites.Count)
        {
            m_playerSpriteRenderer.sprite = m_playerSprites[GameInfoSingleton.Instance.playerSettings.shipLevel];
        }

        // This caps the shipLevel at 2 (wich is 3 but it counts from 0 so in code it says 2)
        if (GameInfoSingleton.Instance.playerSettings.shipLevel >= 2)
        {
            GameInfoSingleton.Instance.playerSettings.shipLevel = 2;
        }

        // This caps the HP at 3
        if (GameInfoSingleton.Instance.playerSettings.playerHP > 3)
        {
            GameInfoSingleton.Instance.playerSettings.playerHP = 3;
        }

        // This runs the MissileAttack funtion when you have missiles avalible
        if(GameInfoSingleton.Instance.playerSettings.missileCount > 0)
        {
            MissileAttack();
        }

        // This sets the missile target (crosshair) on a random enemy when there is a missile in the scene
        // This works hand in hand with the MissileAttack function
        if (FindObjectOfType<HomingMissile>() != null)
        {
            GameObject _randomEnemy = GameObject.FindWithTag("Enemy");
            if (_randomEnemy != null)
            {
                m_crossHairPos = new Vector3(_randomEnemy.transform.position.x, _randomEnemy.transform.position.y, -1);
            }
            if (m_crossHair != null)
            {
                m_crossHair.transform.position = m_crossHairPos;
            }
        }
    }

    // Spawns a new wave, or a boss every 5 waves
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

    // Instantiates the boss
    private void SpawnBoss()
    {
        GameObject boss = Instantiate(m_boss, m_enemySpawnPos, Quaternion.identity);
        m_currentWaveEnemies.Add(boss);
        m_isSpawningWave = false;
    }

    // Checks if a new wave needs to be started and starts one if needed
    private void CheckWaveStatus()
    {
        m_currentWaveEnemies.RemoveAll(enemy => enemy == null);

        if (m_currentWaveEnemies.Count == 0 && !m_isSpawningWave)
        {
            SpawnNewWave();
        }
    }

    // This coroutine actualy spawns the wave, it chooses a random amount of enemys and spawns this amount of enemys
    // Every enemy it spawns is a random enemy and in the EnemyBase script this enemy choses if it is type 1 or 2
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

    // This shoots a missile when m_missileAttack is true
    // This gets set true in the PlayerMovement script and the movement of the missile happens in its own script
    private void MissileAttack()
    {
        HomingMissile[] _missiles = FindObjectsOfType(typeof(HomingMissile)) as HomingMissile[];

        if (m_missileAttack && _missiles.Length == 0)
        {
            FindObjectOfType<AudioManager>().Play("Missile");
            GameObject _missileObj = Instantiate(m_missile, m_player.transform.position, Quaternion.identity);
            GameInfoSingleton.Instance.playerSettings.missileCount--;

            HomingMissile _missileScript = _missileObj.GetComponent<HomingMissile>();

            m_crossHair = Instantiate(m_crossHairPref, m_crossHairPos, Quaternion.identity);

            _missileScript.m_crossHair = m_crossHair;

            m_missileAttack = false;
        }
    }
}
