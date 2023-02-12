using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐릭터의 움직임을 담당하는 클래스
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // 움직임 가속도
    [SerializeField]
    private float moveAcceleration = 70;

    // 회전하는 속도 
    [SerializeField]
    private float rotateSpeed = 7;

    // 즉시 회전하는 속도
    [SerializeField]
    private float immediateRotateSpeed = 35;

    // 최고 가속도
    [SerializeField]
    private float maxVelocity = 5;

    private Vector3 movePower;

    private Rigidbody _rigidbody;

    /// <summary>
    /// 해당 방향으로 가속도만큼 힘을 준다
    /// </summary>
    public void Move(Vector3 direction) 
    {
        movePower = direction * moveAcceleration;
    }

    /// <summary>
    /// 해당 방향으로 rotateSpeed 만큼 보간시켜 회전시킨다
    /// </summary>
    public void SmoothLookAt(Vector3 lookPoint) 
    {
        if (lookPoint == Vector3.zero) 
        {
            return;
        }

        var targetRotation = Quaternion.LookRotation(lookPoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 해당 방향으로 immediate rotate speed 만큼 보간시켜 회전시킨다
    /// </summary>
    public void LookAt(Vector3 lookPoint) 
    {
        var targetRotation = Quaternion.LookRotation(lookPoint);

        if (Quaternion.Angle(targetRotation, transform.rotation) <= 1.0f) {
            transform.rotation = targetRotation;
        } else {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, immediateRotateSpeed * Time.deltaTime);
        }
    }

    protected void Start() 
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate() 
    {
        if ((((_rigidbody.transform.position.x + movePower.normalized.x) 
            * (_rigidbody.transform.position.x + movePower.normalized.x)) 
            + ((_rigidbody.transform.position.z + movePower.normalized.z) 
            * (_rigidbody.transform.position.z + movePower.normalized.z))) > 1600)
            movePower = Vector3.zero;

        _rigidbody.AddForce(movePower);
        

        if (_rigidbody.velocity.x >= maxVelocity) {
            _rigidbody.velocity = new Vector3(maxVelocity, _rigidbody.velocity.y, _rigidbody.velocity.z);
        }

        if (_rigidbody.velocity.y >= maxVelocity) {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, maxVelocity, _rigidbody.velocity.z);
        }

        if (_rigidbody.velocity.z >= maxVelocity) {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y, maxVelocity);
        }

      
    }
}
