using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurSpawner : MonoBehaviour
{

    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject dinosaurBase;
    [SerializeField] List<Dinosaur> dinosaurTypes;

    PopulationManager populationManager;

    private void Awake()
    {
        populationManager = transform.GetComponent<PopulationManager>();
    }

    public void SpawnDinosaur(Dinosaur dinosaur, Vector3 spawnPos)
    {
        GameObject dinosaurInstance = Instantiate(dinosaurBase);
        DinosaurSetup dinosaurSetup = dinosaurInstance.GetComponent<DinosaurSetup>();
        dinosaurSetup.Dinosaur = dinosaur;
        dinosaurSetup.SpawnPos = spawnPos;
        dinosaurSetup.SpawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        dinosaurSetup.enabled = true;

        populationManager.AddDinosaur(dinosaurInstance.transform);
    }

    public List<Dinosaur> GetDinosaurTypes()
    {
        return dinosaurTypes;
    }

}
