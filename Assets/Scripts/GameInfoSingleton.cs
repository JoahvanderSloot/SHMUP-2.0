using UnityEngine;

public class GameInfoSingleton : MonoBehaviour
{
    //Makes a singleton called "GameInfoSingleton" wich i use to reference the PlaterSettings scripteble object
    private static GameInfoSingleton instance;

    public static GameInfoSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameInfoSingleton>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("GameInfoSingleton");
                    instance = singletonObject.AddComponent<GameInfoSingleton>();
                }
            }
            return instance;
        }
    }

    public PlayerSettings playerSettings;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
