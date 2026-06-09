using UnityEngine;
using System.Collections.Generic;

public class EnemyData
{
    public int[] arrayData;
    public int maxArrayLength;
    public EnemyData(int length)
    {
        maxArrayLength = length;
        arrayData = new int[length];

        for(int i = 0; i < length; i++)
        {
            arrayData[i] = Random.Range(1, 100);
        }
    }
    public int[] GetArray()
    {
        return arrayData;
    }
    public bool IsSorted()
    {
        for (int i =0; i< arrayData.Length -1; i++)
        {
            if(arrayData[i] > arrayData[i+1]) return false;
        }
        return true;
    }
}
