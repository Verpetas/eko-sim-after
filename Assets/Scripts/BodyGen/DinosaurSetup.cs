using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DinosaurSetup : MonoBehaviour
{

    [SerializeField] Dinosaur dinosaur;

    Transform root;

    private void Awake()
    {
        root = transform.Find("Wrapper").Find("Root");
    }

    private void Start()
    {
        InitializeCreation();
    }

    void InitializeCreation()
    {
        InitializeBodyPart("LegPair_0");
        if (!dinosaur.bipedal) InitializeBodyPart("LegPair_1");

        InitializeBodyPart("Body");
    }   

    void InitializeBodyPart(string bodyPartName)
    {
        Transform bodyPart = root.Find(bodyPartName);
        bodyPart.gameObject.SetActive(true);
    }

    public Dinosaur GetDinosaur()
    {
        return dinosaur;
    }

}
