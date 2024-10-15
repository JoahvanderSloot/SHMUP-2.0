using UnityEngine;

public class RepairButtons : MonoBehaviour
{
    [SerializeField] RepairScript m_repairScript;

    public void ButtonUp()
    {
        m_repairScript.m_inputSequence.Add(0);
        m_repairScript.CheckInputSequence();
    }

    public void ButtonDown()
    {
        m_repairScript.m_inputSequence.Add(1);
        m_repairScript.CheckInputSequence();
    }

    public void ButtonLeft()
    {
        m_repairScript.m_inputSequence.Add(2);
        m_repairScript.CheckInputSequence();
    }

    public void ButtonRight()
    {
        m_repairScript.m_inputSequence.Add(3);
        m_repairScript.CheckInputSequence();
    }
}
