using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected float health = 100;

    [SerializeField]
    private float destroyInterval = 2.0f;

    protected bool death = false;

    public GameObject DathEffect;
    public GameObject hitEffect;

    public float healthValue { get { return health; } }

    // 해당 캐릭터가 죽었는지 여부를 반환한다
    protected bool IsDeath { get { return death; } }

    /// <summary>
    /// 주어진 데미지를 적용시킨다
    /// 채력이 0 이하로 내려가면 죽인다
    /// </summary>
    /// 뭔가 나이스하게 작성하면 좋겠지만 시간없음으로 오버라이딩으로 구현
    public virtual void TakeHit(float damage, RaycastHit hit) 
    {
        health -= damage;

        if (health <= 0 && !death) 
        {
            Death();
        }
    }

    public void Death()
    {
        death = true;
        if (animator == null) {
            //좀비는 애니메이션이 없을 수 있음
            if (DathEffect != null)
                Instantiate(DathEffect, this.transform);


            // TODO check 
        } else {
            Debug.Log("death trigger");

            if (animator.name == "monster_03")
                if (DathEffect != null)
                    Instantiate(DathEffect, this.transform);

            if (animator.name== "main_character")
                if(DathEffect != null)
                 Instantiate(DathEffect, this.transform);

            animator.SetTrigger("Death");
            OnDeath();
        }

        StartCoroutine(DestroyCharacter());
    }

    public virtual void OnDeath() { }

    private IEnumerator DestroyCharacter()
    {
       

        yield return new WaitForSeconds(destroyInterval);

        if (this.gameObject.tag.Equals("Player") || this.gameObject.tag.Equals("Pet"))
        {
            PlayManager.Instance.stageUI.SetGameOver();
            Time.timeScale = 0;
            Destroy(AudioManager.Instance.gameObject);
        }
        GameObject.Destroy(gameObject);
    }
}
