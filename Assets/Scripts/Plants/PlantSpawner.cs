using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField] Camera worldCamera;
    [SerializeField] List<GameObject> seedTypes;

    VegetationManager vegetationManager;

    private void Awake()
    {
        vegetationManager = transform.GetComponent<VegetationManager>();
    }

    public void SapawnPlant(GameObject seed, Vector3 spawnPos)
    {
        Transform plantInstance = Instantiate(seed, transform).transform;
        plantInstance.position = spawnPos;

        if (seed.name == "PalmSeed")
            vegetationManager.AddPalmTree(plantInstance);
    }

    public List<GameObject> GetSeedTypes()
    {
        return seedTypes;
    }

}
