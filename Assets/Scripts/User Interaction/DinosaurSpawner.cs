using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurSpawner : MonoBehaviour
{

    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject dinosaurBase;
    [SerializeField] List<Dinosaur> dinosaurs;

    PopulationManager populationManager;
    LayerMask groundLayerMask;

    private void Awake()
    {
        populationManager = transform.GetComponent<PopulationManager>();

        int groundLayer = LayerMask.NameToLayer("Ground");
        groundLayerMask |= 1 << groundLayer;
    }

    private void Update()
    {
        HandleSpawnInput();
    }

    void HandleSpawnInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                GameObject dinosaurInstance = Instantiate(dinosaurBase);
                DinosaurSetup dinosaurSetup = dinosaurInstance.GetComponent<DinosaurSetup>();
                dinosaurSetup.Dinosaur = PickRandomDinosaur();
                dinosaurSetup.SpawnPos = hit.point;
                dinosaurSetup.SpawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                dinosaurSetup.enabled = true;

                populationManager.AddDinosaur(dinosaurInstance);
            }
        }
    }

    Dinosaur PickRandomDinosaur()
    {
        return dinosaurs[Random.Range(0, dinosaurs.Count)];
    }

}
