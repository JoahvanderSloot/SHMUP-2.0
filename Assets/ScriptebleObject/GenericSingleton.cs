using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (Instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject _obj = new GameObject();
                    _obj.name = typeof(T).Name;
                    m_instance = _obj.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }
    
    public virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;

            //keeps it when loading new scene (optional)
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
