using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 플레이어를 공격하는 좀비
///
/// GameObject의 Tag랑 Layer를 Enemy로 변경시켜야 한다
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : Character
{

    // 해당 좀피가 플레이어에 데미지를 가하는 간격
    [SerializeField]
    private float hitInterval = 1.0f;

    [SerializeField]
    private float speedMultiply = 1.0f;

    private NavMeshAgent navMesh;

    // 해당 좀비가 쫒을 타겟
    private Transform target;

    // 해당 좀비가 쫒을 타겟
    private Transform pet;

    // 데미지 가하는 코루틴을 보관하는 변수
    private Coroutine hitPlayerCo;

    private Coroutine hitPetCo;

    private Vector3 lastPosition;

    private int score = 0;

    public override void OnDeath() {
        PlayManager.Instance.score += score;
        PlayManager.Instance?.stageUI.UpdateScore();
    }

    protected void Start() 
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = navMesh.speed * 1.5f * speedMultiply;

        if(PlayManager.Instance.player != null)
            target = PlayManager.Instance.player.transform;
        if(PlayManager.Instance.dog != null)
            pet = PlayManager.Instance.dog.transform;

        StartCoroutine(UpdatePath());
        lastPosition = transform.position;
        score = (int) health;
    }

    protected void FixedUpdate() 
    {
        if (animator) {
            var diff = Vector3.Distance(lastPosition, transform.position);
            lastPosition = transform.position;

            animator.SetFloat("Speed", (diff < 0.01f ? 0.0f : diff));
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && object.ReferenceEquals(hitPlayerCo, null))  {
            hitPlayerCo = StartCoroutine(HitPlayer());
        }  else if (collision.gameObject.tag == "Pet" && object.ReferenceEquals(hitPetCo, null)) {
            hitPetCo = StartCoroutine(HitPet());
        }
    }

    protected void OnCollisionExit(Collision collision)  {
        if (collision.gameObject.tag == "Player" && !object.ReferenceEquals(hitPlayerCo, null))  {
            StopCoroutine(hitPlayerCo);
        } else if (collision.gameObject.tag == "Pet" && !object.ReferenceEquals(hitPetCo, null)) {
            StopCoroutine(hitPetCo);
        }
    }

    /// <summary>
    /// 타겟의 위치를 가지고 길을 계속 갱신한다
    /// </summary>
    private IEnumerator UpdatePath() 
    {
        var wait = new WaitForSeconds(1.0f);

        while (target != null && pet != null && !IsDeath)
        {
            var targetDist = Vector3.Distance(transform.position, target.transform.position);
            var petDist = Vector3.Distance(transform.position, pet.transform.position);

            var dest = targetDist < petDist ? target.transform.position : pet.transform.position;

            var targetPosition = new Vector3(dest.x, 0.0f, dest.z);
            navMesh.SetDestination(targetPosition);

            yield return wait; 
        }
    }

    /// <summary>
    /// hit inverval만큼의 간격으로 플레이어에게 데미지를 준다
    /// </summary>
    private IEnumerator HitPlayer()
    {
        var wait = new WaitForSeconds(hitInterval);

        var character = target.gameObject.GetComponent<Character>();
        while (!IsDeath) 
        {
            character.TakeHit(10, new RaycastHit());
            PlayManager.Instance.stageUI.SetUIPlayerHP(PlayManager.Instance.player.healthValue);
            yield return wait;
        }
    }

    /// <summary>
    /// hit inverval만큼의 간격으로 플레이어에게 데미지를 준다
    /// </summary>
    private IEnumerator HitPet() {
        var wait = new WaitForSeconds(hitInterval);

        var dog = pet.gameObject.GetComponent<Character>();
        while (!IsDeath) {
            dog.TakeHit(15, new RaycastHit());
            PlayManager.Instance.stageUI.SetUIDogHP(PlayManager.Instance.dog.healthValue);
            yield return wait;
        }
    }

    public override void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;
        if(hitEffect!=null)
            Instantiate(hitEffect, this.transform);
        if (health <= 0 && !death)
        {
            Death();
        }
    }
}
