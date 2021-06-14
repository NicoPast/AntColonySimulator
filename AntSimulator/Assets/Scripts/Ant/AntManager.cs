using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class AntManager : MonoBehaviour
{
    [SerializeField]
    AntHill antH;

    [SerializeField]
    private List<Ant> ants;

    private void Update()
    {
        //for(var i = 0; i < transform.childCount; i++)
        //{
        //    ants.Add(transform.GetChild(i).GetComponent<Ant>());
        //}
        var antDataArray = new NativeArray<Ant.Data>(ants.Count, Allocator.TempJob);
        for (var i = 0; i < ants.Count; i++)
        {
            antDataArray[i] = new Ant.Data(ants[i], transform, antH);
        }
        var job = new AntUpdateJob
        {
            AntDataArray = antDataArray
        };
        var jobHandle = job.Schedule(ants.Count, 1);
        jobHandle.Complete();
        antDataArray.Dispose();
    }
}