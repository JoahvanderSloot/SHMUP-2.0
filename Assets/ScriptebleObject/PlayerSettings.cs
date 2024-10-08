using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo.asset", menuName = "Game Info", order = 0)]
public class PlayerSettings : GenericScriptableSingleton<PlayerSettings>
{
    public bool audio;
    public int playerHP;
    public string IGN;
    public int score;
    public int wave;
}
    