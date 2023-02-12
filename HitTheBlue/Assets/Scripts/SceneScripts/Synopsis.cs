using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Synopsis : Loading
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        PlayManager.Instance.SetStageNumber(1);
    }

    protected override void LoadText()
    {
        var text = TextManager.Instance.textData.synopsisText;
        
        if(!object.ReferenceEquals(text, null))
            textUI.text = text.text;

        textScroll = TextScrollCoroutine();
        StartCoroutine(textScroll);
    }
}
