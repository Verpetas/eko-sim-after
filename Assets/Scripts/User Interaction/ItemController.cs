using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ItemController : MonoBehaviour
{
    [SerializeField] TMP_Text selectedItemType;
    [SerializeField] TMP_Text selectedItem;

    [SerializeField] Camera worldCamera;

    [SerializeField] DinosaurSpawner dinosaurSpawner;
    [SerializeField] PlantSpawner plantSpawner;
    [SerializeField] WateringCan wateringCan;

    LayerMask groundLayerMask;

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
            HandleMousePress();

        if (Input.GetMouseButton(0))
            HandleMouseHold();
    }

    void HandleMousePress()
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
        }
    }

    void HandleMouseHold()
    {
        Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            if (itemTypeSelection == ItemType.Tool)
            {
                wateringCan.UseWateringCan(hit.point);
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
            selectedItem.text = "Watering Can";
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
                selectedItem.text = "Watering Can";
            }
        }
    }

}
