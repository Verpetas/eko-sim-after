using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class ObjectScatterer : MonoBehaviour
{

    [SerializeField] ScatterObject[] objectTypes;

    LayerMask groundLayerMask;

    public void ScatterObjects(Vector2 mapCorner, int chunkCount)
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        groundLayerMask |= 1 << groundLayer;

        for (int typeIndex = 0; typeIndex < objectTypes.Length; typeIndex++)
        {
            Transform objectOfTypeHolder = new GameObject(objectTypes[typeIndex].typeName).transform;
            objectOfTypeHolder.parent = transform;

            int spawnedInstanceCount = 0;
            int instanceToSpawnCount = objectTypes[typeIndex].instanceCountPerChunk * chunkCount;

            while (spawnedInstanceCount < instanceToSpawnCount)
            {
                Vector3 rayStart = new Vector3(UnityEngine.Random.Range(0, mapCorner.x), 500f, UnityEngine.Random.Range(0, mapCorner.y));
                Ray ray = new Ray(rayStart, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
                {
                    if (hit.point.y > objectTypes[typeIndex].spawnHeightInterval.x && hit.point.y < objectTypes[typeIndex].spawnHeightInterval.y)
                    {
                        int subtypeIndex = UnityEngine.Random.Range(0, objectTypes[typeIndex].variants.Length);
                        Transform objectInstance = Instantiate(objectTypes[typeIndex].variants[subtypeIndex], objectOfTypeHolder).transform;
                        objectInstance.position = hit.point;
                        objectInstance.up = hit.normal;
                        objectInstance.localScale *= 9;
                        objectInstance.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));

                        spawnedInstanceCount++;
                    }
                }
            }
        }
    }

}


[System.Serializable]
public class ScatterObject
{
    public string typeName;
    public int instanceCountPerChunk;
    public Vector2 spawnHeightInterval;
    public GameObject[] variants;
}