using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DinosaurSpawner : MonoBehaviour
{

    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject dinosaurBase;
    [SerializeField] List<Dinosaur> dinosaurTypes;

    PopulationManager populationManager;
    float seaLevel;

    private void Awake()
    {
        populationManager = transform.GetComponent<PopulationManager>();

        TerrainGenerator terrain = GameObject.FindWithTag("MapGenerator").GetComponent<TerrainGenerator>();
        seaLevel = terrain.SeaLevel;
    }

    public void SpawnDinosaur(Dinosaur dinosaur, Vector3 spawnPos)
    {
        if (spawnPos.y > seaLevel)
        {
            Transform dinosaurInstance = Instantiate(dinosaurBase).transform;
            DinosaurSetup dinosaurSetup = dinosaurInstance.GetComponent<DinosaurSetup>();
            dinosaurSetup.Dinosaur = dinosaur;
            dinosaurSetup.SpawnPos = spawnPos;
            dinosaurSetup.SpawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            dinosaurSetup.enabled = true;

            dinosaurInstance.parent = transform;

            populationManager.AddDinosaur(dinosaurInstance);
        }
    }

    public List<Dinosaur> GetDinosaurTypes()
    {
        return dinosaurTypes;
    }

}
