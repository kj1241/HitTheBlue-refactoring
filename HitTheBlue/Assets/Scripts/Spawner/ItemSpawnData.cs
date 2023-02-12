using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemSpawnData 
{
    // 해당 스폰 데이터의 아이템 데이터
    public Item prefab;

    // 스폰할수 있는 확률
    // 0 = 스폰하지 않음
    // 1 = 무조건 스폰함
    [Range(0, 1)]
    public float spawnChance;
}
