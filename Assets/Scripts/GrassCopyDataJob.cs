using UnityEngine;
using System.Collections;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

public struct GrassCopyDataJob : IJob
{
    [ReadOnly]
    public NativeList<GrassData> mTempData;
    public NativeList<GrassData> mPersistentData;


    public void Execute()
    {
        //mPersistentData = mTempData.AsArray();
        mPersistentData.AddRange(mTempData);
    }
}
