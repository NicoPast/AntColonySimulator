using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public struct AntUpdateJob : IJobParallelFor
{
    public NativeArray<Ant.Data> AntDataArray;

    public void Execute(int index)
    {
        var data = AntDataArray[index];
        data.Update();
        AntDataArray[index] = data;
    }
}
