using UnityEngine;
using Unity.Entities;

[RequiresEntityConversion]
//[AddComponentMenu("DOTS Samples/IJobChunk/Rotation Speed")]
[ConverterVersion("joy", 1)]
public class GrassInstanceComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public uint InstanceID;

    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new GrassInstanceComponent { InstanceID = InstanceID };
        dstManager.AddComponentData(entity, data);
    }
}
