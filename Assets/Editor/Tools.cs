using UnityEngine;
using UnityEditor;
using Unity.Mathematics;

public class Tools : ScriptableObject
{

    [MenuItem("Tools/RandomGrassInstanceID")]
    static void RandomGrassInstanceID()
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1,1000000000));
        //random.InitState();

        GrassInstanceComponentAuthoring[] grassDatas= GameObject.FindObjectsOfType<GrassInstanceComponentAuthoring>();
        Debug.Log("Grass num: " + grassDatas.Length);
        foreach(var d in grassDatas)
        {
            d.InstanceID = random.NextUInt(1023);
            d.transform.position = new Vector3(random.NextFloat(20), 0, random.NextFloat(20));
            EditorUtility.SetDirty(d);
        }
    }
}
