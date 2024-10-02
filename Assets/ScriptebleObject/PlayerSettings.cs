using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo.asset", menuName = "Game Info", order = 0)]
public class PlayerSettings : GenericScriptableSingleton<PlayerSettings>
{
    public bool audio;
}
