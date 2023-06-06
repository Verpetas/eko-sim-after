using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField] Camera worldCamera;
    [SerializeField] List<GameObject> seedTypes;

    VegetationManager vegetationManager;
    float seaLevel;

    private void Awake()
    {
        vegetationManager = transform.GetComponent<VegetationManager>();

        TerrainGenerator terrain = GameObject.FindWithTag("MapGenerator").GetComponent<TerrainGenerator>();
        seaLevel = terrain.SeaLevel;
    }

    public void SapawnPlant(GameObject seed, Vector3 spawnPos)
    {
        if (spawnPos.y > seaLevel)
        {
            Transform plantInstance = Instantiate(seed, transform).transform;
            plantInstance.position = spawnPos;

            if (seed.name == "PalmSeed")
                vegetationManager.AddPalmTree(plantInstance);
        }
    }

    public List<GameObject> GetSeedTypes()
    {
        return seedTypes;
    }

}
