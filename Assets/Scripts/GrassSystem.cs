using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public struct GrassData
{
    public float3 Position;
    public uint InstanceID;
}

public class GrassSystem : SystemBase
{
    private EntityQuery query;
    private NativeList<GrassData> mGrassData;
    private NativeList<GrassData> mTempGrassData;
    private JobHandle mJobHandle;
    private bool mJobStarted = false;

    private GrassData[] mGrassArray;
    private ComputeBuffer positionBuffer;

    protected override void OnCreate()
    {
        base.OnCreate();

        mGrassData = new NativeList<GrassData>(1024, Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        //赋给类的成员变量

        if(mJobStarted)
        {
            //mJobHandle.Complete();


            //Debug.Log(mGrassData.Length);

            //             if(mGrassArray == null)
            //                 mGrassArray = mGrassData.ToArray();

            //Debug.Log("  1mTempGrassData.Length: " + mTempGrassData.Length);
            if (positionBuffer == null)
            {
                //Debug.Log("mTempGrassData.Length: " + mTempGrassData.Length);
                positionBuffer = new ComputeBuffer(mTempGrassData.Length, 16);
                positionBuffer.SetData(mTempGrassData.AsArray());
            }

            mTempGrassData.Dispose();
        }

        NativeList<GrassData> tempList = new NativeList<GrassData>(0,Allocator.TempJob);
        mTempGrassData = tempList;
        //mGrassData.Clear();

        //1. culling.
        Entities
            .WithName("GrassSystem")
            .WithStoreEntityQueryInField(ref query)
            .ForEach((in Unity.Transforms.Translation position, in GrassInstanceComponent grassInstance) =>
            {
                //                 rotation.Value = math.mul(
                //                     math.normalize(rotation.Value),
                //                     quaternion.AxisAngle(math.up(), rotationSpeed.RadiansPerSecond * deltaTime));
                //testArray.Append(grassInstance.InstanceID);

                uint test = grassInstance.InstanceID;
                GrassData d = new GrassData()
                {
                    Position = position.Value,
                    InstanceID = grassInstance.InstanceID,
                };
                tempList.Add(d);
            })
            .Schedule();


//         //2. 
//         GrassCopyDataJob job = new GrassCopyDataJob();
//         job.mTempData = tempList;
//         job.mPersistentData = mGrassData;
//         mJobHandle = job.Schedule(Dependency);
        mJobStarted = true;
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();
        mJobHandle.Complete();
        if (mGrassData.IsCreated) mGrassData.Dispose();
        if (mTempGrassData.IsCreated) mTempGrassData.Dispose();
    }

    public GrassData[] GetGrassData()
    {
        return mGrassArray;
    }

    public NativeArray<GrassData> GetGrassDataNativeArray()
    {
        return mGrassData.AsArray();
    }

    public ComputeBuffer GetComputeBuffer()
    {
        return positionBuffer;
    }
}
