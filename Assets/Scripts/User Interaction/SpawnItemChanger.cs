using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class SpawnItemChanger : MonoBehaviour
{
    [SerializeField] TMP_Text selectedItemType;
    [SerializeField] TMP_Text selectedItem;

    [SerializeField] Camera worldCamera; 

    LayerMask groundLayerMask;

    DinosaurSpawner dinosaurSpawner;
    PlantSpawner plantSpawner;

    List<Dinosaur> dinosaurTypes;
    List<GameObject> seedTypes;

    enum ItemType { Dinosaur, Seed, Tool };
    ItemType itemTypeSelection;

    int dinosaurSelectionIndex;
    int seedSelectionIndex;

    private void Awake()
    {
        itemTypeSelection = ItemType.Dinosaur;
        dinosaurSelectionIndex = seedSelectionIndex = 0;

        dinosaurSpawner = GameObject.FindWithTag("PopulationManager").GetComponent<DinosaurSpawner>();
        plantSpawner = GameObject.FindWithTag("VegetationManager").GetComponent<PlantSpawner>();

        dinosaurTypes = dinosaurSpawner.GetDinosaurTypes();
        seedTypes = plantSpawner.GetSeedTypes();

        int groundLayer = LayerMask.NameToLayer("Ground");
        groundLayerMask |= 1 << groundLayer;
    }

    private void Start()
    {
        selectedItem.text = dinosaurTypes[dinosaurSelectionIndex].name;
    }

    void Update()
    {
        HandleItemTypeSwitchInput();
        HandleItemSwitchInput();
        HandleItemUseInput();
    }

    void HandleItemUseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                if (itemTypeSelection == ItemType.Dinosaur)
                {
                    dinosaurSpawner.SpawnDinosaur(dinosaurTypes[dinosaurSelectionIndex], hit.point);
                }
                if (itemTypeSelection == ItemType.Seed)
                {
                    plantSpawner.SapawnPlant(seedTypes[seedSelectionIndex], hit.point);
                }
                if (itemTypeSelection == ItemType.Tool)
                {
                    // use tool
                }
            }
        }
    }

    void HandleItemTypeSwitchInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemTypeSelection = ItemType.Dinosaur;
            selectedItem.text = dinosaurTypes[dinosaurSelectionIndex].name;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemTypeSelection = ItemType.Seed;
            selectedItem.text = seedTypes[seedSelectionIndex].name;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            itemTypeSelection = ItemType.Tool;
            selectedItem.text = "No Tools Yet";
        }

        selectedItemType.text = itemTypeSelection.ToString() + " Selected:";
    }

    void HandleItemSwitchInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (itemTypeSelection == ItemType.Dinosaur)
            {
                dinosaurSelectionIndex++;
                if (dinosaurSelectionIndex >= dinosaurTypes.Count) dinosaurSelectionIndex = 0;

                selectedItem.text = dinosaurTypes[dinosaurSelectionIndex].name;
            }

            if (itemTypeSelection == ItemType.Seed)
            {
                seedSelectionIndex++;
                if (seedSelectionIndex >= seedTypes.Count) seedSelectionIndex = 0;

                selectedItem.text = seedTypes[seedSelectionIndex].name;
            }

            if (itemTypeSelection == ItemType.Tool)
            {
                selectedItem.text = "No Tools Yet";
            }
        }
    }

}
