using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float SmoothTime = 0.3f;

    [SerializeField]
    private Vector3 offset;

    // 해당 카메라가 쫒을 타겟
    private Transform target;

    protected void Start() {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate() {
        if(target == null)
            return;

        Vector3 velocity = new Vector3();

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, SmoothTime);

    }
}
