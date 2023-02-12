using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Character
{
    [SerializeField]
    protected int basePower = 100;
    public GameObject GunEffect;

    public int randomPower
    {
        get
        {
            return basePower;
        }
    }

    [SerializeField]
    private float speed = 10;

    private float lifetime = 2.0f;

    [SerializeField]
    private LayerMask collisionMask;

    public void SetSpeed(float newSpeed) 
    {
        speed = newSpeed;
    }

    protected void Update() 
    {
        var distance = speed * Time.deltaTime;
        CheckCollisions(distance);
        transform.Translate(Vector3.forward * distance);


        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f) {
             Destroy(this.gameObject);
        }
    }
    
    protected void OnHitObject(RaycastHit hit)
    {
        var character = hit.collider.GetComponent<Character>();
        if (character != null) 
        {
            if (GunEffect != null)
                Instantiate(GunEffect, this.transform); 

            character.TakeHit(randomPower, hit);

        }
        GameObject.Destroy(gameObject);
    }

    private void CheckCollisions(float distance)
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, collisionMask, QueryTriggerInteraction.Collide)) 
        {
            OnHitObject(hit);
        }
    }

}
