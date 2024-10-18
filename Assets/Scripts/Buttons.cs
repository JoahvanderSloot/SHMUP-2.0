using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void PointerEnter()
    {
        transform.localScale = new Vector2(1.05f, 1.05f);
    }

    public void PointerExit()
    {
        transform.localScale = new Vector2(1f, 1f);
    }

    public void StartGame()
    {
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
        SceneManager.LoadScene("Start");
    }

    public void Continue()
    {
        PlayerMovement m_playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        m_playerMovement.m_IsPaused = false;
    }

    public void SkipRepair()
    {
        SceneManager.LoadScene("GameOver");
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
          Application.OpenURL("https://joahvds.itch.io/wa-lch");
        #endif
    }
}
