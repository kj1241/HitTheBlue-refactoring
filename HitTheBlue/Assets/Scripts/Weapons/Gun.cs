using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    [SerializeField]
    private Vector3 positionOffset;
    
    [SerializeField]
    private Vector3 rotationOffset;

    [SerializeField]
    private GameObject attachSocket;

    // 총알이 발사될 위치를 가진 Transform
    [SerializeField]
    protected Transform muzzle;

    // 총알의 가속도
    [SerializeField]
    protected float bulletVelocity = 10;

    // 총알 Prefab
    [SerializeField]
    protected Bullet bullet;

    [SerializeField]
    private int currentBullet = 0;

    [SerializeField]
    private bool isFiring = false;
    private int maxBullet = 0;

    public GameObject GunEffect;

    public int CurrentBullet { get { return currentBullet; } }
    public int MaxBullet { get { return maxBullet; } }
    
    // 해당 총이 발사중인지 여부를 반환한다
    public bool IsFiring { get { return isFiring; } }

    // 해당 총을 발사할 수 있는지 여부를 반환한다
    public bool CanAttack {
        get {
            return currentBullet > 0;
        }
    }
    
    const int BULLET_PER_MAGAZINE = 20;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = attachSocket.transform;
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset);
    }

    /// <summary>
    /// 주어진 양만큼 총알을 증가시킨다
    /// </summary>
    public void AddBullet(int amount)
    {
        currentBullet += amount;
    }

    public void AddMaxBullet(int amount)
    {
        maxBullet += amount;
        PlayManager.Instance?.stageUI.SetUIBullet(currentBullet, maxBullet);
    }

    public void Reload()
    {
        PlayManager.Instance?.stageUI.ActivateReloadInfoUI(false, false);
        if(maxBullet <= 0)
            return;
        if(maxBullet < BULLET_PER_MAGAZINE)
        {
            currentBullet += maxBullet;
            maxBullet = 0;
        }
        else
        {
            currentBullet += BULLET_PER_MAGAZINE;
            maxBullet -= BULLET_PER_MAGAZINE;
        }
            
        PlayManager.Instance?.stageUI.SetUIBullet(currentBullet, maxBullet);
        AudioManager.Instance.Play("reloding_P");
    }

    public void BeginFire()
    {
        isFiring = true;
    }

    public bool Fire(Quaternion rotation)
    {
        isFiring = false;

        if (!CanAttack) {
            return false;
        }

        currentBullet -= 1;

        AudioManager.Instance.Play("gunshot");

        if(currentBullet == 0)
        {
            PlayManager.Instance?.stageUI.ActivateReloadInfoUI(true, (maxBullet == 0));
        }

        if(GunEffect!=null)
            Instantiate(GunEffect, muzzle.transform);


        PlayManager.Instance?.stageUI.SetUIBullet(currentBullet, maxBullet);

        var newBullet = Instantiate(bullet, muzzle.position, rotation) as Bullet;
        newBullet.SetSpeed(bulletVelocity);

        return true;

    }

}
