using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VegetationManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnedPalmTrees;
    [SerializeField] PlantsAutoGrowToggle plantsAutoGrowToggle;
    public bool plantsAutoGrow = false;

    List<CoconutTree> coconutTrees;

    private void Awake()
    {
        spawnedPalmTrees = new List<Transform>();
        coconutTrees = new List<CoconutTree>();
    }

    private void Update()
    {
        HandlePlantAoutGrowToggle();
    }

    void HandlePlantAoutGrowToggle()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            plantsAutoGrow = !plantsAutoGrow;
            plantsAutoGrowToggle.ToggleText(plantsAutoGrow);
        }
    }

    public void AddPalmTree(Transform palmTree)
    {
        spawnedPalmTrees.Add(palmTree);
    }

    public void AddCoconutTree(CoconutTree coconutTree)
    {
        coconutTrees.Add(coconutTree);
    }

    public void RemoveFromCocoTreePool(CoconutTree coconutTree)
    {
        coconutTrees.Remove(coconutTree);
    }

    public List<CoconutTree> CoconutTrees
    {
        get { return coconutTrees; }
    }
}


public class CoconutTree
{
    Transform tree;
    List<Transform> coconuts;

    VegetationManager vegetationManager;

    public CoconutTree(Transform tree, List<Transform> coconuts)
    {
        this.tree = tree;
        this.coconuts = coconuts;

        vegetationManager = GameObject.FindWithTag("VegetationManager").GetComponent<VegetationManager>();
    }

    public Transform PickRandomCoconut()
    {
        int randomCoconutIndex = Random.Range(0, coconuts.Count);
        Transform pickedCoconut = coconuts[randomCoconutIndex];

        coconuts.Remove(pickedCoconut);
        if (coconuts.Count < 1) vegetationManager.RemoveFromCocoTreePool(this);

        return pickedCoconut;
    }

    public Transform Tree
    {
        get { return tree; }
    }

    public List<Transform> Coconuts
    {
        get { return coconuts; }
    }
}