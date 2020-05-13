using System;
using Unity.Entities;

[Serializable]
public struct GrassInstanceComponent : IComponentData
{
    public uint InstanceID;
}
