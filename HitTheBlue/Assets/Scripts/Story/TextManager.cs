using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public TextScriptableObject textData = null;
    private static TextManager m_instance;
    public static TextManager Instance
    {
        get
        {
            if(m_instance == null)
            {
                GameObject go = new GameObject("TextManager");
                TextManager manager = go.AddComponent<TextManager>();
                manager.textData = Resources.Load<TextScriptableObject>("Texts/TextData");
            }

            return m_instance;
        }
    }


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_instance = this;
        var obj = FindObjectsOfType<TextManager>();
        if (obj.Length == 1)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if(m_instance!=null&& m_instance)
            m_instance = null;
    }
}
