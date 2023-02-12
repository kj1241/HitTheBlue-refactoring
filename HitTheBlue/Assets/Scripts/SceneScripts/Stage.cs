using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    Player player;
    [SerializeField]
    Dog dog;
    [SerializeField]
    GameObject uiPrefab;


    void Awake()
    {
        if(player == null || dog == null)
            return;

        GameObject go = Instantiate(uiPrefab);
        PlayManager.Instance.InitCharacter(player, dog);
        StageUI stageUI = go.GetComponent<StageUI>();
        if(stageUI != null)
        {
            stageUI.InitHP(player, dog);
            PlayManager.Instance.InitUI(stageUI);

            PlayManager.Instance.score = 0;
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (PlayManager.Instance != null)
        {
            PlayManager.Instance.ClearCharacter();
            PlayManager.Instance.ClearUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnApplicationQuit()
    {
        Destroy(PlayManager.Instance.gameObject);
    }
}
