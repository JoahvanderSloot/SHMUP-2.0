using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RepairScript : MonoBehaviour
{
    [SerializeField] List<int> m_randomSequence = new List<int>();
    [SerializeField] List<GameObject> m_buttons;
    [SerializeField] TextMeshProUGUI m_correctIncorrect;
    [SerializeField] Color m_color;
    public List<int> m_inputSequence;
    Coroutine m_showSequence;
    bool m_showedSequence = false;
    PlayerMovement m_playerMovement;
    float m_deathTimer;
    bool m_isIncorrect;

    private void Start()
    {
        m_deathTimer = 0;

        GameObject _player = GameObject.FindWithTag("Player");
        m_playerMovement = _player.GetComponent<PlayerMovement>();

        // Chose a random sequence
        int _maxSequence = 6;
        if (GameInfoSingleton.Instance.playerSettings.wave <= _maxSequence)
        {
            for (int i = 0; i < GameInfoSingleton.Instance.playerSettings.wave; i++)
            {
                m_randomSequence.Add(Random.Range(0, 4));
            }
        }
        else
        {
            for (int i = 0; i < _maxSequence; i++)
            {
                m_randomSequence.Add(Random.Range(0, 4));
            }
        }

        // Start showing the sequence
        if (m_showSequence == null && gameObject.activeInHierarchy)
        {
            m_showSequence = StartCoroutine(ShowSequence());
            m_playerMovement.Pause();
            GameInfoSingleton.Instance.playerSettings.isRepairing = true;
            Time.timeScale = 0;
        }
    }

    private void Update()
    {
        // When it is showing the sequence you cant click the buttons
        // When it is done you can click them again
        if (!m_showedSequence)
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                Button _button = m_buttons[i].GetComponent<Button>();
                _button.interactable = false;
               
            }
            m_correctIncorrect.text = "memorize";
            m_correctIncorrect.color = Color.white;
        }
        else
        {
            for (int i = 0; i < m_buttons.Count; i++)
            {
                Button _button = m_buttons[i].GetComponent<Button>();
                _button.interactable = true;
            }
        }

        // If you got a button incorrect you die and the GameOver scene gets loaded
        if (m_isIncorrect)
        {
            m_deathTimer += 2 + Time.deltaTime;
            if (m_deathTimer >= 10)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    public void CheckInputSequence()
    {
        bool sequenceCorrect = true;

        for (int i = 0; i < m_inputSequence.Count; i++)
        {
            if (m_inputSequence[i] != m_randomSequence[i])
            {
                sequenceCorrect = false;
                break;
            }
        }

        // Every time you click a button it checks if is is the correct one in the sequense
        // If this sequnce is correct it tells you that and you can click the next one
        // But if it is false it tells you that and sets m_isIncorrect to true
        if (sequenceCorrect)
        {
            FindObjectOfType<AudioManager>().Play("Correct");
            m_correctIncorrect.text = "Correct";
            m_correctIncorrect.color = Color.green;

            if (m_inputSequence.Count == m_randomSequence.Count)
            {
                GameInfoSingleton.Instance.playerSettings.playerHP++;
                GameInfoSingleton.Instance.playerSettings.isRepairing = false;
                m_playerMovement.Pause();
                gameObject.SetActive(false);
                m_inputSequence.Clear();

                List<GameObject> _targets = new List<GameObject>();

                _targets.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
                _targets.AddRange(GameObject.FindGameObjectsWithTag("Bullet"));

                foreach (GameObject _target in _targets)
                {
                    if (_target.GetComponent<Boss>() == null)
                    {
                        if (_target.CompareTag("Enemy"))
                        {
                            EnemyBase _enemyBase = _target.GetComponent<EnemyBase>();
                            Instantiate(GameInfoSingleton.Instance.playerSettings.explotion, _target.transform.position, Quaternion.identity);
                        }
                        Destroy(_target);
                    }
                }
            }
        }
        else
        {
            FindObjectOfType<AudioManager>().Play("Incorrect");
            m_correctIncorrect.text = "Incorrect";
            m_correctIncorrect.color = Color.red;
            m_isIncorrect = true;
        }
    }

    // This shows the sequence with a little time in between so u can remember it
    IEnumerator ShowSequence()
    {
        for (int i = 0; i < m_randomSequence.Count; i++)
        {
            yield return new WaitForSecondsRealtime(1f);

            FindObjectOfType<AudioManager>().Play("Beep");
            Image _buttonIMG = m_buttons[m_randomSequence[i]].GetComponent<Image>();
            _buttonIMG.color = m_color;

            yield return new WaitForSecondsRealtime(0.5f);

            _buttonIMG.color = Color.white;
        }

        m_showSequence = null;
        m_showedSequence = true;
    }
}
