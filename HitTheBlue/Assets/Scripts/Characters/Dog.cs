using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Dog : Character
{
    [SerializeField]
    private float speedMultiply = 0.8f;

    [SerializeField]
    private float minDistance = 5.0f;

    private NavMeshAgent navMesh;

    // 해당 좀비가 쫒을 타겟
    private Transform target;

    protected void Start() {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = navMesh.speed * 1.5f * speedMultiply;

        if(PlayManager.Instance.player != null)
            target = PlayManager.Instance.player.transform;

        StartCoroutine(UpdatePath());
    }

    private void FixedUpdate() {
        if (navMesh != null && target != null) {
            var targetPosition = new Vector3(target.position.x, 0.0f, target.position.z);
            var sqrDistance = (transform.position - targetPosition).sqrMagnitude;
            navMesh.SetDestination(targetPosition);
            navMesh.isStopped = (sqrDistance <= minDistance);
        }
    }

    /// <summary>
    /// 타겟의 위치를 가지고 길을 계속 갱신한다
    /// </summary>
    private IEnumerator UpdatePath() {
        var wait = new WaitForSeconds(1.0f);

        while (target != null && !IsDeath) {
            var targetPosition = new Vector3(target.position.x, 0.0f, target.position.z);
            navMesh.SetDestination(targetPosition);

            yield return wait;
        }
    }
}
