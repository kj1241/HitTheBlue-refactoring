using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageUI : MonoBehaviour
{
    // Player
    [SerializeField]
    Slider playerHP;

    [SerializeField]
    Slider playerHPAfterimage;

    // Dog
    [SerializeField]
    Slider dogHP;

    [SerializeField]
    Slider dogHPAfterImage;

    [SerializeField]
    Text ammoStatus;
    [SerializeField]
    GameObject gameOverUI;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    CanvasGroup canvasGroup;
    [SerializeField]
    GameObject reloadInfoUI;
    const string AMMO_STATUS = "{0}/{1}";
    const string NO_AMMO = "NO AMMO";
    const string RELOAD_INFO = "Press <color=yellow>R</color> to reload";
    bool isLoadingScenen;

    // Start is called before the first frame update
    void Start()
    {
        isLoadingScenen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitHP(Player player, Dog dog)
    {
        playerHPAfterimage.value = playerHPAfterimage.maxValue = playerHP.value = playerHP.maxValue = player.healthValue;
        dogHPAfterImage.value = dogHPAfterImage.maxValue = dogHP.value = dogHP.maxValue = dog.healthValue;

        dogHPAfterImage.minValue = playerHPAfterimage.minValue = playerHP.minValue = dogHP.minValue = 0f;
    }

    public void SetUIPlayerHP(float hp)
    {
        playerHP.value = hp;

        /* TODO: 시간나면 잔상 연출효과 */
    }

    public void SetUIDogHP(float hp)
    {
        dogHP.value = hp;

        /* TODO: 시간나면 잔상 연출효과 */
    }

    public void SetUIBullet(int bullet, int maxBullet)
    {
        ammoStatus.text = string.Format(AMMO_STATUS, bullet, maxBullet);
    }

    public void UpdateScore()
    {
        scoreText.text = string.Format("점수: {0}", PlayManager.Instance?.score);
    }

    public void SetGameOver()
    {
        reloadInfoUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void LoadTitleMenu()
    {
        if(!isLoadingScenen)
            StartCoroutine(LoadTitleMenuCoroutine());
    }

    public void ActivateReloadInfoUI(bool isOn, bool isNoAmmo)
    {
        var textUI = reloadInfoUI.GetComponent<Text>();
        if(textUI != null)
            textUI.text = (isNoAmmo)?NO_AMMO:RELOAD_INFO;

        reloadInfoUI.SetActive(isOn);
    }

    IEnumerator LoadTitleMenuCoroutine()
    {
        isLoadingScenen = true;
        float timer = 0f;
        float limit = 1f;
        var sceneLoad = SceneManager.LoadSceneAsync("TitleMenu");
        sceneLoad.allowSceneActivation = false;

        while(timer < limit)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, Mathf.Clamp(timer, 0f, limit) / limit);
            yield return new WaitForEndOfFrame();
        }

        while (sceneLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        isLoadingScenen = false;
        sceneLoad.allowSceneActivation = true;
    }
}
