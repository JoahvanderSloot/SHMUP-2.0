using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "GameInfo.asset", menuName = "Game Info", order = 0)]
public class PlayerSettings : GenericScriptableSingleton<PlayerSettings>
{
    public int playerHP;
    public string IGN;
    public int score;
    public int wave;
    public int shipLevel;
    public bool shieldIsActive;
    public GameObject explotion;
}
    