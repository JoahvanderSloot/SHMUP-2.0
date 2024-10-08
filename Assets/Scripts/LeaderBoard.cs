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
        if (!m_NamesList.Contains(PlayerSettings.Instance.IGN))
        {
            m_NamesList.Add(PlayerSettings.Instance.IGN);
            m_ScoreList.Add(PlayerSettings.Instance.score);
            CheckForRanking();
        }
        else
        {
            int _currentName = m_NamesList.IndexOf(PlayerSettings.Instance.IGN);
            if (PlayerSettings.Instance.score > m_ScoreList[_currentName])
            {
                m_ScoreList[_currentName] = PlayerSettings.Instance.score;
                CheckForRanking();
            }
        }

        foreach (Transform child in m_Canvas.transform)
        {
            Destroy(child.gameObject);
        }

        int displayCount = Mathf.Min(10, m_NamesList.Count);
        for (int i = 0; i < displayCount; i++)
        {
            int _rank = i + 1;
            TextMeshProUGUI _newText = Instantiate(m_LeaderBoard, m_Canvas.transform);
            RectTransform _rectTransform = _newText.GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = new Vector2(-290, 220 - i * 15);
            _newText.text = "#" + _rank + " - " + m_ScoreList[i] + ": " + m_NamesList[i];
        }

        int playerRank = m_NamesList.IndexOf(PlayerSettings.Instance.IGN) + 1;
        if (playerRank > 10)
        {
            TextMeshProUGUI playerText = Instantiate(m_LeaderBoard, m_Canvas.transform);
            RectTransform playerRectTransform = playerText.GetComponent<RectTransform>();
            playerRectTransform.anchoredPosition = new Vector2(-290, 220 - displayCount * 15 - 10);
            playerText.text = "#" + playerRank + " - " + PlayerSettings.Instance.score + ": " + PlayerSettings.Instance.IGN;
        }

        SaveData();
    }

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

    public void SaveData()
    {
        LeaderboardData data = new LeaderboardData();
        data.NamesList = m_NamesList;
        data.ScoreList = m_ScoreList;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

            m_NamesList = data.NamesList;
            m_ScoreList = data.ScoreList;
        }
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<string> NamesList;
    public List<int> ScoreList;
}
