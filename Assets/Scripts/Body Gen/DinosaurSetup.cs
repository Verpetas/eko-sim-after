using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DinosaurSetup : MonoBehaviour
{

    [SerializeField] Dinosaur dinosaur;

    Transform root;

    Vector3 spawnPos;
    Quaternion spawnRot;

    private void Awake()
    {
        root = transform.Find("Wrapper").Find("Root");
    }

    private void Start()
    {
        InitializeBodyCreation();
    }

    void InitializeBodyCreation()
    {
        InitializeBodyPartCreation("LegPair_0");
        if (!dinosaur.bipedal) InitializeBodyPartCreation("LegPair_1");

        InitializeBodyPartCreation("Body");
    }

    void InitializeBodyPartCreation(string bodyPartName)
    {
        Transform bodyPart = root.Find(bodyPartName);
        bodyPart.gameObject.SetActive(true);
    }

    public void AddCollider(float radius, float height)
    {
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.radius = radius;
        collider.height = height;
    }

    public void AssignSpawnPosition()
    {
        transform.position = spawnPos + Vector3.up * 10f;
        transform.rotation = spawnRot;
    }

    public Dinosaur Dinosaur
    {
        get { return dinosaur; }
        set { dinosaur = value; }
    }

    public Vector3 SpawnPos
    {
        get { return spawnPos; }
        set { spawnPos = value; }
    }

    public Quaternion SpawnRot
    {
        get { return spawnRot; }
        set { spawnRot = value; }
    }

}
