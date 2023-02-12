using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 조종 가능한 플레이어
///
/// GameObject의 Tag를 Player로 변경시켜야 한다
/// </summary>
[RequireComponent(typeof(PlayerController))]
public class Player : Character
{
    // 시작시 플레이어가 보유랑 총알의 개수
    public int startBulletCount = 20;

    // 총알을 발사하는 최대 각도
    [SerializeField]
    protected float maxAngleOfBulletFire = 1.0f;

    [SerializeField]
    protected Gun gun;

    private PlayerController controller;

    private Camera viewCamera;

    /// <summary>
    /// 주어진 양만큼 총알을 증가시킨다. 탄창 안의 총알과 분리돼있음.
    /// </summary>
    public void AddMaxBullet(int amount)
    {
        gun.AddMaxBullet(amount);
        PlayManager.Instance?.stageUI.ActivateReloadInfoUI(false, false);
    }

    protected void Start()
    {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;

        gun.AddBullet(startBulletCount);
        gun.AddMaxBullet(startBulletCount);
    }

    protected void Update()
    {
        if (IsDeath) {
            controller.Move(Vector3.zero);
            return;
        }

        // 키보드의 입력을 받아 캐릭터에게 가속도를 추가한다
        var horz = Input.GetAxisRaw("Horizontal");
        var vert = Input.GetAxisRaw("Vertical");
        var moveInput = new Vector3(horz, 0, vert);

        controller.Move(moveInput.normalized);
        if (animator) {
            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetFloat("Horizontal", horz);
            animator.SetFloat("Vertical", vert);
        }

        if (!gun.IsFiring) 
        {
            controller.SmoothLookAt(moveInput.normalized);
        }

        // Fire 버튼을 누르면 발사를 진행한다
        if (Input.GetButtonDown("Fire")) 
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.R) && gun.MaxBullet > 0)
            Reload(); 

        if(this.transform.position.y <-5f)
        {
            Death();
        }


    }

    /// <summary>
    /// 마우스 위치로 총알을 발사한다
    /// </summary>
    private void Attack()
    {
        if (gun.IsFiring) {
            return;
        }

        var ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) 
        {
            // 총알 발사가 성공적으로 진행됨
            gun.BeginFire();

            var point = hit.point;

            // 해당 플레이어 위치와 상대적으로 만들기
            point -= transform.position;

            // 회전각도 구하는데 y는 필요 없다
            point.y = 0;

            StartCoroutine(RotationAndShoot(point));
        }
    }

    private void Reload()
    {
        gun.Reload();
    }

    /// <summary>
    /// 주어진 방향을 바라본 후 총알이 발사된다
    /// </summary>
    private IEnumerator RotationAndShoot(Vector3 point)
    {
        var wait = new WaitForFixedUpdate();
        var lookRotation = Quaternion.LookRotation(point);
        do {
            controller.LookAt(point);
            if (Quaternion.Angle(lookRotation, transform.rotation) > 1.0f) {
                yield return wait;
            } else {
                break;
            }
        } while (true);

        if (!IsDeath) {
            // 총 발사에 성공하면 발사 에니메이션 실행
            if (gun.Fire(lookRotation)) {
                if (animator) {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }

    public override void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;
        if (hitEffect != null && this != null)
            Instantiate(hitEffect, this.transform);
        if (health <= 0 && !death)
        {
            Death();
        }
    }
}
