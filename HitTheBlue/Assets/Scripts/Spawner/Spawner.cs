using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Spawner : MonoBehaviour
{
    [SerializeField]
    private SpawnDataScriptableObject spawnData;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool alwaysSpawnItem = true;
    
    [SerializeField]
    private bool alwaysSpawnMonster = true;

    [SerializeField]
    private int maxTryCount = 25;

    [SerializeField]
    private int[] spawnCountList;

    private BoxCollider boxCollider;

    private Coroutine itemSpawnCo = null;

    private Coroutine monsterSpawnCo = null;

    public SpawnDataScriptableObject SpawnData {
        get { return spawnData; }
    }

    public bool isSpawnFinished {
        get {
            bool result = true;
            for (int i = 0; i < spawnCountList.Length; i++) {
                if (spawnData.monsterDataList[i].maxSpawnCount < 0) {
                    result = false;
                    continue;
                }

                if (spawnData.monsterDataList[i].maxSpawnCount > spawnCountList[i]) {
                    result = false;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 스폰 데이터를 설정하고 정보들을 초기화한다
    /// </summary>
    /// <param name="data"></param>
    public void SetSpawnData(SpawnDataScriptableObject data) {
        spawnData = data;

        if (itemSpawnCo != null) {
            StopCoroutine(itemSpawnCo);
            itemSpawnCo = null;
        }

        if (spawnData.itemDataList.Length > 0) {
            itemSpawnCo = StartCoroutine(ItemSpawn());
        }

        if (monsterSpawnCo != null) {
            StopCoroutine(monsterSpawnCo);
            monsterSpawnCo = null;
        }

        if (spawnData.monsterDataList.Length > 0) {
            monsterSpawnCo = StartCoroutine(MonsterSpawn());
        }

        spawnCountList = new int[spawnData.monsterDataList.Length];
        for (int i = 0; i < spawnCountList.Length; i++) {
            spawnCountList[i] = 0;
        }
    }

    protected void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        SetSpawnData(SpawnData);
    }

    /// <summary>
    /// 아이템을 랜덤한 간격으로 랜덤한 위치에 소환시킨다
    /// </summary>
    protected IEnumerator ItemSpawn()
    {
        while (true) {
            float spawnInterval = spawnData.itemSpawnInterval.GetRandomPoint();
            yield return new WaitForSeconds(spawnInterval);

            int tryCount = 0;
            while (true) {
                Vector3 randomPosition = GetRandomPoint();
                var random = Random.Range(0, spawnData.itemDataList.Length);
                var itemSpawnData = spawnData.itemDataList[random];

                var point = GetGroundPoint(randomPosition);

                if (IsValidPoint(point)) {
                    var spawnChance = Random.Range(0.0f, 1.0f) < itemSpawnData.spawnChance;
                    if (spawnChance) {
                        var item = Instantiate(itemSpawnData.prefab, point, Quaternion.identity) as Item;
                        break;
                    }
                }

                if (!alwaysSpawnItem) {
                    break;
                }

                if (++tryCount > maxTryCount) {
                    break;
                }
            }
        }

        yield return null;
    }

    protected IEnumerator MonsterSpawn()
    {
        while (true) {
            float spawnInterval = spawnData.monsterSpawnInterval.GetRandomPoint();
            yield return new WaitForSeconds(spawnInterval);

            int tryCount = 0;
            while (true) {
                Vector3 randomPosition = GetRandomPoint();
                var random = Random.Range(0, spawnData.monsterDataList.Length);
                var monsterSpawnData = spawnData.monsterDataList[random];
                if (monsterSpawnData.maxSpawnCount > 0 && monsterSpawnData.maxSpawnCount <= spawnCountList[random]) {
                    if (++tryCount > maxTryCount) {
                        break;
                    }

                    break;
                }

                var point = GetGroundPoint(randomPosition);

                if (IsValidPoint(point)) {
                    var spawnChance = Random.Range(0.0f, 1.0f) < monsterSpawnData.spawnChance;
                    if (spawnChance) {
                        Instantiate(monsterSpawnData.prefab, point, Quaternion.identity);
                        spawnCountList[random] += 1;
                        break;
                    }
                }

                if (!alwaysSpawnMonster) {
                    break;
                }

                if (++tryCount > maxTryCount) {
                    break;
                }
            }
        }

        yield return null;
    }

    private Vector3 GetRandomPoint()
    {
        var bounds = boxCollider.bounds;
        return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
    }

    /// <summary>
    /// 주어진 좌표를 기준으로 땅 좌표를 찾는다
    /// </summary>
    private Vector3 GetGroundPoint(Vector3 point)
    {
        var ray = new Ray(point, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, groundMask, QueryTriggerInteraction.Collide)) {
            return hit.point;
        }

        return point;
    }

    /// <summary>
    /// 해당 위치에 오브젝트를 소환할 수 있는지 여부를 반화
    /// </summary>
    private bool IsValidPoint(Vector3 point)
    {
        var hits = Physics.OverlapSphere(point, 1);
        return hits.Length <= 2;
    }
}
