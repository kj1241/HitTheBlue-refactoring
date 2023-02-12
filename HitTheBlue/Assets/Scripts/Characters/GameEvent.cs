using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute]
public class GameEvent : ScriptableObject
{
    private List<GameEventListiner> listeners = new List<GameEventListiner>(); //�����ʸ� ��Ƶ� ����Ʈ 

    public void Raise()//�� �Լ��� ����Ǹ� �ٷ����� ����Ʈ�� ����� �����ʵ��� ���� �����ͼ�
                       //���� GameEventListnerŬ������ listeners ������ �Լ��� OnEventRised()�� ����
    {
        for (int i = listeners.Count - 1; i >= 0; --i)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListiner listener)// ������ ��� �Լ�
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListiner listener)
    {
        listeners.Remove(listener);
    }
}
