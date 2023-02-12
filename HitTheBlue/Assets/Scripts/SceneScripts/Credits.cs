using UnityEngine.UI;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField]
    TextAsset text;
    [SerializeField]
    Text textUI;
    [SerializeField]
    float scrollTime = 0.8f;
    private bool timerStart = false;
    private float timer;

    void Start()
    {
        textUI.text = text.text;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if(!timerStart)
        {
            timer += Time.unscaledDeltaTime;

            if(scrollTime < timer)
                timerStart = true;
        }
    }
    void FixedUpdate()
    {
        if(timerStart)
        {
            Vector3 creditPosition = textUI.transform.localPosition;
            textUI.transform.localPosition = new Vector3(creditPosition.x, creditPosition.y + 1f, creditPosition.z);
        }
    }
}
