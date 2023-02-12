using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        var obj = FindObjectsOfType<DontDestroy>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else if(obj.Length > 1)
        {
            for(int i=0; i< obj.Length-1; ++i)
                Destroy(obj[i].gameObject);

            DontDestroyOnLoad(this.gameObject);
        }
    }
    
}
