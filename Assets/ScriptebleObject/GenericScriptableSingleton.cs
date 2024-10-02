using UnityEngine;

public class GenericScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                // Find an existing instance in the project (optional)
                m_instance = Resources.FindObjectsOfTypeAll<T>()[0];

                // If not found, create a new instance
                if (m_instance == null)
                {
                    m_instance = CreateInstance<T>();
                }
            }
            return m_instance;
        }
    }

    // Optional: if needed, use Awake for initialization logic
    protected virtual void OnEnable()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
        }
    }
}
