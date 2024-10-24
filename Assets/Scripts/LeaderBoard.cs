using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_LeaderBoard;
    Canvas m_Canvas;
    List<string> m_NamesList = new List<string>();
    List<int> m_ScoreList = new List<int>();

    private void Start()
    {
        m_Canvas = GetComponent<Canvas>();
        LoadData();
        UpdateScore();
    }

    private void UpdateScore()
    {
        // This removes every player which did not enter a IGN (name)
        for (int i = m_NamesList.Count - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(m_NamesList[i]))
            {
                m_NamesList.RemoveAt(i);
                m_ScoreList.RemoveAt(i);
            }
        }

        // This adds a new player when their name is not found in the list, and the checks for ranking
        // (which puts the player at its deseved spot)
        if (!m_NamesList.Contains(GameInfoSingleton.Instance.playerSettings.IGN))
        {
            if (!string.IsNullOrEmpty(GameInfoSingleton.Instance.playerSettings.IGN))
            {
                m_NamesList.Add(GameInfoSingleton.Instance.playerSettings.IGN);
                m_ScoreList.Add(GameInfoSingleton.Instance.playerSettings.score);
                CheckForRanking();
            }
        }
        // If the player is already in the list it checks if the new score is higher then the old one
        // if it is it sets the new score as the players highscore and ranks the leaderboard again
        else
        {
            int _currentName = m_NamesList.IndexOf(GameInfoSingleton.Instance.playerSettings.IGN);
            if (GameInfoSingleton.Instance.playerSettings.score > m_ScoreList[_currentName])
            {
                m_ScoreList[_currentName] = GameInfoSingleton.Instance.playerSettings.score;
                CheckForRanking();
            }
        }

        // Destroy the old leaderboard from the canvast so it can make a new one with the updated scores
        foreach (Transform child in m_Canvas.transform)
        {
            Destroy(child.gameObject);
        }

        // Instantates the text objects and displays the top 10
        int displayCount = Mathf.Min(10, m_NamesList.Count);
        for (int i = 0; i < displayCount; i++)
        {
            int _rank = i + 1;
            TextMeshProUGUI _newText = Instantiate(m_LeaderBoard, m_Canvas.transform);
            RectTransform _rectTransform = _newText.GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = new Vector2(-290, 220 - i * 15);
            _newText.text = "#" + _rank + " - " + m_ScoreList[i] + ": " + m_NamesList[i];
        }

        // If the current player is not in the top 10 it sets it below the top 10 list and displays its ranking
        // This is so you can always find your name on the leaderboard even if you are not in the top 10 currently
        int playerRank = m_NamesList.IndexOf(GameInfoSingleton.Instance.playerSettings.IGN) + 1;
        if (playerRank > 10)
        {
            TextMeshProUGUI playerText = Instantiate(m_LeaderBoard, m_Canvas.transform);
            RectTransform playerRectTransform = playerText.GetComponent<RectTransform>();
            playerRectTransform.anchoredPosition = new Vector2(-290, 220 - displayCount * 15 - 10);
            playerText.text = "#" + playerRank + " - " + GameInfoSingleton.Instance.playerSettings.score + ": " + GameInfoSingleton.Instance.playerSettings.IGN;
        }

        // Save the leaderboard
        SaveData();
    }

    // Load the saved leavderboard from a json it gets saved in every time after updating
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

            m_NamesList = data.NamesList;
            m_ScoreList = data.ScoreList;

            for (int i = m_NamesList.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(m_NamesList[i]))
                {
                    m_NamesList.RemoveAt(i);
                    m_ScoreList.RemoveAt(i);
                }
            }
        }
    }

    // This checks the whole scoreboard and sets it from highest score to lowest score in a combined list
    // Then puts everything back in the normal list in the correct the order
    private void CheckForRanking()
    {
        List<(string name, int score)> combinedList = new List<(string, int)>();

        for (int i = 0; i < m_NamesList.Count; i++)
        {
            combinedList.Add((m_NamesList[i], m_ScoreList[i]));
        }

        combinedList.Sort((a, b) => b.score.CompareTo(a.score));

        m_NamesList.Clear();
        m_ScoreList.Clear();

        foreach (var item in combinedList)
        {
            m_NamesList.Add(item.name);
            m_ScoreList.Add(item.score);
        }
    }

    // Saves the scoreboard on a json file
    public void SaveData()
    {
        LeaderboardData data = new LeaderboardData();
        data.NamesList = m_NamesList;
        data.ScoreList = m_ScoreList;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<string> NamesList;
    public List<int> ScoreList;
}
