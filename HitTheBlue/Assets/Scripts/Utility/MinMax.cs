using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinMax
{
    public float min;

    public float max;

    public float GetRandomPoint()
    {
        return Random.Range(min, max);
    }
}

