using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Text", order = 2)]
public class TextScriptableObject : ScriptableObject
{
    public TextAsset synopsisText;
    public TextAsset endingText;
    public TextAsset[] loadingText;
    /* 크레딧은 크레딧씬에서 따로 관리 */
}
