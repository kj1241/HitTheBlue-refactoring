using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class GameEventListiner : MonoBehaviour
{
    public GameEvent EventSO; //오브젝트에 접근하기 위한 변수
    public UnityEvent Response; // 유니티 이벤트에 접근하기 위한 함수

    private void OnEnable()
    {
        EventSO.RegisterListener(this); //현재 클래스를 등록
    }

    private void OnDisable() 
    {
        EventSO.UnRegisterListener(this); //현재 클래스를 등록
    }

    public void OnEventRaised()// 이벤트 발생
    {
        Response.Invoke();// unityEvnet 형식의 변수 Response에 접근해서 Invoke()를 실행,리스너들에게 이벤트를 발생하는 함수
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
