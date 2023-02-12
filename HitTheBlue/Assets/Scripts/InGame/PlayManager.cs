using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoSingleton<PlayManager>
{
    public Player player = null;
    public Dog dog = null;
    public StageUI stageUI = null;
    private int m_stageNum = 1;
    public int stageNum
    {
        get { return m_stageNum; }
    }

    public int score = 0;

    public void SetNextStage()
    {
        this.m_stageNum = Mathf.Clamp(this.m_stageNum + 1, 1, 5);
    }

    /* 쓸 일은 없겠지만 테스트용 */
    public void SetStageNumber(int stageIndex)
    {
        this.m_stageNum = Mathf.Clamp(stageIndex, 1, 5);
    }

    public void InitCharacter(Player player, Dog dog)
    {
        this.player = player;
        this.dog = dog;
    }

    public void ClearCharacter()
    {
        this.player = null;
        this.dog = null;
    }

    public void InitUI(StageUI stageUI)
    {
        this.stageUI = stageUI;
    }

    public void ClearUI()
    {
        this.stageUI = null;
    }
 
}
