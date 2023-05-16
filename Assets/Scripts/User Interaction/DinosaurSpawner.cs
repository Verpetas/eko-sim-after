using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurSpawner : MonoBehaviour
{

    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject dinosaur;

    LayerMask groundLayerMask;

    private void Update()
    {
        HandleSpawnInput();

        int groundLayer = LayerMask.NameToLayer("Ground");
        groundLayerMask |= 1 << groundLayer;
    }

    void HandleSpawnInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                GameObject dinosaurInstance = Instantiate(dinosaur);
                DinosaurSetup dinosaurSetup = dinosaurInstance.GetComponent<DinosaurSetup>();
                dinosaurSetup.SpawnPos = hit.point;
                dinosaurSetup.SpawnRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                dinosaurSetup.enabled = true;
            }
        }
    }

}
