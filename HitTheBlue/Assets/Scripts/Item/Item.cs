using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemScriptableObject itemData;

    public ItemScriptableObject ItemData { 
        get { 
            return itemData; 
        } 
    }

    protected void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            var player = collider.gameObject.GetComponent<Player>();
            player.AddMaxBullet(itemData.bulletAdd);

            GameObject.Destroy(gameObject);
        }
    }
}
