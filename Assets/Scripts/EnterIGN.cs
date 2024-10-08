using TMPro;
using UnityEngine;

public class EnterIGN : MonoBehaviour
{
    [SerializeField] TMP_InputField m_enterIGN;

    private void Start()
    {
        m_enterIGN = GetComponent<TMP_InputField>();
    }

    public void SetIGN()
    {
        PlayerSettings.Instance.IGN = m_enterIGN.text.ToString();
    }
}
