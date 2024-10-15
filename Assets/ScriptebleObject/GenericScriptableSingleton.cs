#if UNITY_EDITOR
using UnityEditor;
#endif

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
                // Try to find the asset in the project
                T[] assets = Resources.FindObjectsOfTypeAll<T>();
                if (assets.Length > 0)
                {
                    m_instance = assets[0];
                }
                #if UNITY_EDITOR
                else
                {
                    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
                    if (guids.Length > 0)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                        m_instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    }
                }
                #endif
                // If still not found, create a new instance
                if (m_instance == null)
                {
                    m_instance = CreateInstance<T>();
                }
            }
            return m_instance;
        }
    }
}
