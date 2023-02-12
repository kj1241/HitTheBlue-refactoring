using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MonsterSpawnData
{
    // 해당 몬스터의 프리팹
    public Character prefab;

    // 해당 몬스터가 최대로 스폰되는 숫자
    public int maxSpawnCount;

    // 스폰할수 있는 확률
    // 0 = 스폰하지 않음
    // 1 = 무조건 스폰함
    [Range(0.0f, 1.0f)]
    public float spawnChance;
}
