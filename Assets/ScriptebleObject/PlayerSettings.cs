using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo.asset", menuName = "Game Info", order = 0)]
public class PlayerSettings : ScriptableObject
{
    public int playerHP;
    public string IGN;
    public int score;
    public int wave;
    public int shipLevel;
    public bool shieldIsActive;
    public GameObject explotion;
    public bool isRepairing;
}
    