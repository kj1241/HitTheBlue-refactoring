using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class GameEventListiner : MonoBehaviour
{
    public GameEvent EventSO; //������Ʈ�� �����ϱ� ���� ����
    public UnityEvent Response; // ����Ƽ �̺�Ʈ�� �����ϱ� ���� �Լ�

    private void OnEnable()
    {
        EventSO.RegisterListener(this); //���� Ŭ������ ���
    }

    private void OnDisable() 
    {
        EventSO.UnRegisterListener(this); //���� Ŭ������ ���
    }

    public void OnEventRaised()// �̺�Ʈ �߻�
    {
        Response.Invoke();// unityEvnet ������ ���� Response�� �����ؼ� Invoke()�� ����,�����ʵ鿡�� �̺�Ʈ�� �߻��ϴ� �Լ�
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
