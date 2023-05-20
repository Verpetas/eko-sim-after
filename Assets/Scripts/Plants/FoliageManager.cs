using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageManager : MonoBehaviour
{
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject palmSeed;
    [SerializeField] List<Transform> spawnedPalmTrees;

    LayerMask groundLayerMask;

    private void Awake()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        groundLayerMask |= 1 << groundLayer;
    }

    void Update()
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
                Transform palmTreeInstance = Instantiate(palmSeed, transform).transform;
                palmTreeInstance.position = hit.point;

                spawnedPalmTrees.Add(palmTreeInstance);
            }
        }
    }
}
