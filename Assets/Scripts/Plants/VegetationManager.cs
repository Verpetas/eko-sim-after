using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnedPalmTrees;
    [SerializeField] List<Transform> coconutTrees;

    [SerializeField] List<Transform> availableFood;

    private void Awake()
    {
        spawnedPalmTrees = new List<Transform>();
        coconutTrees = new List<Transform>();
    }

    public void AddPalmTree(Transform palmTree)
    {
        spawnedPalmTrees.Add(palmTree);
    }

    public void AddCoconutTree(Transform palmTree)
    {
        coconutTrees.Add(palmTree);
    }

    public void AddToFood(List<Transform> addedFood)
    {
        availableFood.AddRange(addedFood);
    }

    public void RemoveFoodFromAvailable(Transform foodInstance)
    {
        availableFood.Remove(foodInstance);
    }

    public List<Transform> AvailableFood
    {
        get { return availableFood; }
    }

}