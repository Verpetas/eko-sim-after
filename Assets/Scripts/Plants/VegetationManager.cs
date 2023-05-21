using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnedPalmTrees;

    private void Awake()
    {
        spawnedPalmTrees = new List<Transform>();
    }

    public void AddPalmTree(Transform palmTree)
    {
        spawnedPalmTrees.Add(palmTree);
    }
}