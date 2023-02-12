using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField]
    protected Button skipButton;
    [SerializeField]
    protected CanvasGroup canvasGroup;
    [SerializeField]
    protected Text textUI;
    [SerializeField]
    protected float loadingTime = 5f;
    [SerializeField]
    protected float scrollSpeed = 3f;
    protected AsyncOperation stageLoad;
    protected IEnumerator startLoading = null, textScroll = null;
    protected bool timerStart = false;
    protected bool isTextCouroutine = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    protected virtual void Start()
    {
        startLoading = StartLoading(false);
        StartCoroutine(startLoading);

        LoadText();
    }

    protected virtual void OnDestroy()
    {
        //if(!object.ReferenceEquals(textScroll, null))
        //    StopCoroutine(textScroll);
    }

    protected virtual IEnumerator StartLoading(bool isSkip)
    {
        stageLoad = SceneManager.LoadSceneAsync("Stage1");//string.Format("Stage{0}", PlayManager.Instance.stageNum));
        stageLoad.allowSceneActivation = false;

        if(!isSkip)
        {
            yield return new WaitForSecondsRealtime(loadingTime);
            skipButton.gameObject.SetActive(false);

            float timer = 0f;
            float limit = 1f;
            while(timer < limit)
            {
                timer += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp(timer, 0f, limit) / limit);
                yield return new WaitForEndOfFrame();
            }
        }

        while (stageLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        
        stageLoad.allowSceneActivation = true;
    }

    protected virtual IEnumerator SkipCoroutine()
    {
        if(startLoading != null)
            StopCoroutine(startLoading);

        float timer = 0f;
        float limit = 1f;
        while(timer < limit)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp(timer, 0f, limit) / limit);
            yield return new WaitForEndOfFrame();
        }

        if(stageLoad != null)
        {
            while (stageLoad.progress < 0.9f)
            {
                yield return new WaitForEndOfFrame();
            }
            
            stageLoad.allowSceneActivation = true;
        }
        else
        {
            yield return StartCoroutine(StartLoading(true));
        }
    }

    public virtual void SkipAction()
    {
        skipButton.gameObject.SetActive(false);
        StartCoroutine(SkipCoroutine());
    }

    protected virtual void LoadText()
    {
        var texts = TextManager.Instance.textData.loadingText;
        var index = (PlayManager.Instance.stageNum - 1);
        
        if(index >= 0 && texts.Length > index)
            textUI.text = texts[index].text;

        textScroll = TextScrollCoroutine();
        StartCoroutine(textScroll);
    }

    protected virtual IEnumerator TextScrollCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        timerStart = true;
    }

    protected virtual void FixedUpdate()
    {
        if(timerStart)
        {
            Vector3 creditPosition = textUI.transform.localPosition;
            textUI.transform.localPosition = new Vector3(creditPosition.x, creditPosition.y + (1f * scrollSpeed), creditPosition.z);
        }
    }
}
