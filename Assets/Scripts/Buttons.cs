using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    [SerializeField] List<Sprite> m_audioImages;

    private void Start()
    {
        Button _button = GetComponent<Button>();
        if (m_audioImages.Count != 0)
        {
            if (GameInfoSingleton.Instance.playerSettings.audio)
            {
                _button.image.sprite = m_audioImages[0];
            }
            else
            {
                _button.image.sprite = m_audioImages[1];
            }
        }
    }

    public void PointerEnter()
    {
        FindObjectOfType<AudioManager>().Play("ButtonHover");
        transform.localScale = new Vector2(1.05f, 1.05f);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }

    public void StartGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
        GameInfoSingleton.Instance.playerSettings.score = 0;
        GameInfoSingleton.Instance.playerSettings.wave = 1;
        GameInfoSingleton.Instance.playerSettings.playerHP = 3;
        GameInfoSingleton.Instance.playerSettings.shipLevel = 0;
        GameInfoSingleton.Instance.playerSettings.isRepairing = false;
        GameInfoSingleton.Instance.playerSettings.missileCount = 3;
    }

    public void MainMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("Start");
    }

    public void Continue()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        PlayerMovement m_playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        m_playerMovement.m_IsPaused = false;
    }

    public void SkipRepair()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("GameOver");
    }

    public void Controls()
    {
        FindObjectOfType<AudioManager>().Play("ButtonPress");
        SceneManager.LoadScene("Controls");
    }

    public void AudioButton()
    {
        GameInfoSingleton.Instance.playerSettings.audio = !GameInfoSingleton.Instance.playerSettings.audio;
        Button _button = GetComponent<Button>();

        if (GameInfoSingleton.Instance.playerSettings.audio)
        {
            FindObjectOfType<AudioManager>().Play("ButtonPress");
            _button.image.sprite = m_audioImages[0];
        }
        else
        {
            _button.image.sprite = m_audioImages[1];
        }
    }

    public void QuitGame()
    {
        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
        #endif
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE)
            Application.Quit();
        #elif (UNITY_WEBGL)
            Application.OpenURL("https://joahvds.itch.io/space-fighter");
        #endif
    }
}
 