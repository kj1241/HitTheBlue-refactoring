using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnData", order = 5)]
public class SpawnDataScriptableObject : ScriptableObject
{
    // 아이템 소환 간격을 정할 랜덤 값의 최소 최대 값
    public MinMax itemSpawnInterval;

    // 스폰할 아이템 들 
    public ItemSpawnData[] itemDataList;

    // 몬스터 소환 간격을 정할 랜덤 값의 최소 최대 값
    public MinMax monsterSpawnInterval;

    // 스폰할 몬스터 들 
    public MonsterSpawnData[] monsterDataList;
}
