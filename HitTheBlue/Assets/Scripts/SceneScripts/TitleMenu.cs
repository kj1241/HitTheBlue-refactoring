using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TitleMenu : MonoBehaviour
{
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    GameObject settings;
    [SerializeField]
    Dropdown resolutionDropdown;
    [SerializeField]
    bool isFullScreenMode = false;
    bool isLoadingScenen;

    private AsyncOperation sceneLoad = null;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        resolutionDropdown.value = (resolutionDropdown.options.Count - 1);
        resolutionDropdown.onValueChanged.AddListener(SetResolutionDropdown);
        isLoadingScenen = false;
    }

    public void StartGame()
    {
        if(!isLoadingScenen)
            StartCoroutine(WaitScene("Synopsis"));
    }

    public void Credits()
    {
        if(!isLoadingScenen)
            StartCoroutine(WaitScene("Credits"));
    }

    public void Settings()
    {
        settings.SetActive(true);
    }

    public void BackToMenu()
    {
        settings.SetActive(false);
    }

    public void Exit()
    {
        if (!isLoadingScenen)
            StartCoroutine(WaitTimer(
                delegate 
                {
                    Application.Quit();
                }
        ));
    }

    IEnumerator WaitTimer(Action action)
    {
        float timer = 0f;
        float limit = 1f;
        while(timer < limit)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp(timer, 0f, limit) / limit);
            yield return new WaitForEndOfFrame();
        }

        if(action != null)
            action();
    }
    
    IEnumerator WaitScene(string sceneName)
    {
        isLoadingScenen = true;
        float timer = 0f;
        float limit = 1f;
        sceneLoad = SceneManager.LoadSceneAsync(sceneName);
        sceneLoad.allowSceneActivation = false;

        while(timer < limit)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp(timer, 0f, limit) / limit);
            yield return new WaitForEndOfFrame();
        }

        while (sceneLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        isLoadingScenen = false;
        sceneLoad.allowSceneActivation = true;
    }


    // options
    public void SetResolutionButton(bool isLeft)
    {
        int count = resolutionDropdown.options.Count;
        int newIndex = (((isLeft)?-1:1) + resolutionDropdown.value + count) % count;
        resolutionDropdown.value = newIndex;
    }

    public void SetResolutionDropdown(int index)
    {
        List<Dropdown.OptionData> options = resolutionDropdown.options;
        string option = options[index % options.Count].text;
        var array = option.Split('x');
        Vector2Int resolution = new Vector2Int();
        if(array.Length > 1)
        {
            int buffer = 0;
            if(int.TryParse(array[0], out buffer))
                resolution.x = buffer;
            if(int.TryParse(array[1], out buffer))
                resolution.y = buffer;

            SetResolution(resolution);
        }
    }

    void SetResolution(Vector2Int resolution)
    {
        Screen.SetResolution(resolution.x, resolution.y, isFullScreenMode);
    }

    public void ToggleFullScreen(bool isOn)
    {
        isFullScreenMode = isOn;

        if(resolutionDropdown.options.Count > 0)
        {
            var selected = resolutionDropdown.options[resolutionDropdown.value];
            string option = selected.text;
            var array = option.Split('x');
            Vector2Int resolution = new Vector2Int();
            if(array.Length > 1)
            {
                int buffer = 0;
                if(int.TryParse(array[0], out buffer))
                    resolution.x = buffer;
                if(int.TryParse(array[1], out buffer))
                    resolution.y = buffer;
            
                SetResolution(resolution);
            }
        }
    }

    public void SetSound(bool isOn)
    {
        AudioListener.volume = (isOn)?1f:0f;
    }
}
