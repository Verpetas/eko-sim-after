using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnedPalmTrees;
    [SerializeField] List<Transform> coconutTrees;

    private void Awake()
    {
        spawnedPalmTrees = new List<Transform>();
        coconutTrees = new List<Transform>();
    }

    public void AddPalmTree(Transform palmTree)
    {
        spawnedPalmTrees.Add(palmTree);
    }

    public void AddCoconutTree(Transform palmTree)
    {
        coconutTrees.Add(palmTree);
    }
}