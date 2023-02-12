using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute]
public class GameEvent : ScriptableObject
{
    private List<GameEventListiner> listeners = new List<GameEventListiner>(); //리스너를 담아둘 리스트 

    public void Raise()//이 함수가 실행되면 바로위에 리스트에 저장된 리스너들을 전부 가저와서
                       //각각 GameEventListner클래스인 listeners 변수의 함수중 OnEventRised()를 실행
    {
        for (int i = listeners.Count - 1; i >= 0; --i)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(GameEventListiner listener)// 리스너 등록 함수
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListiner listener)
    {
        listeners.Remove(listener);
    }
}
