using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurPair : MonoBehaviour
{
    [SerializeField] Transform dinosaurFirst;
    [SerializeField] Transform dinosaurSecond;

    [SerializeField] bool dinosaurFirstReady = false;
    [SerializeField] bool dinosaurSecondReady = false;

    Rigidbody rbFirst;
    Rigidbody rbSecond;

    Dinosaur propertiesFirst;
    Dinosaur propertiesSecond;

    DinosaurSpawner dinosaurSpawner;

    private void Start()
    {
        rbFirst = dinosaurFirst.GetComponent<Rigidbody>();
        rbSecond = dinosaurSecond.GetComponent<Rigidbody>();

        propertiesFirst = dinosaurFirst.GetComponent<DinosaurSetup>().Dinosaur;
        propertiesSecond = dinosaurSecond.GetComponent<DinosaurSetup>().Dinosaur;

        dinosaurSpawner = GetComponent<DinosaurSpawner>();

        StartRoute();
        StartCoroutine(CheckBothReady());
    }

    void StartRoute()
    {
        Unit unitInstanceFist = dinosaurFirst.GetComponent<Unit>();
        Unit unitInstanceSecond = dinosaurSecond.GetComponent<Unit>();

        unitInstanceFist.SetTarget(dinosaurSecond, SetReady);
        unitInstanceSecond.SetTarget(dinosaurFirst, SetReady);
    }

    IEnumerator CheckBothReady()
    {
        while (true)
        {
            rbFirst.velocity = Vector3.zero;
            rbSecond.velocity = Vector3.zero;

            if (dinosaurFirstReady && dinosaurSecondReady)
                CreateNewDinosaur();

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreateNewDinosaur()
    {
        //Dinosaur newDinosaurProperties = DetermineDinosaurProperties();
        //Vector3 spawnPos = (dinosaurFirst.position + dinosaurSecond.position) / 2f;

        //dinosaurSpawner.SpawnDinosaur(newDinosaurProperties, spawnPos);
    }

    //Dinosaur DetermineDinosaurProperties()
    //{
    //    float heredityRatio;
    //    int boneCount = propertiesFirst.spineBends.Length;

    //    Dinosaur newProperties = propertiesFirst;

    //    for (int i = 0; i < boneCount; i++)
    //    {
    //        heredityRatio = Random.Range(0.25f, 0.75f);
    //        newProperties.spineBends[i] = propertiesFirst.spineBends[i] * heredityRatio + propertiesSecond.spineBends[i] * (1 - heredityRatio);

    //        heredityRatio = Random.Range(0.25f, 0.75f);
    //        newProperties.spineWidths[i].x = propertiesFirst.spineWidths[i].x * heredityRatio + propertiesSecond.spineWidths[i].x * (1 - heredityRatio);

    //        heredityRatio = Random.Range(0.25f, 0.75f);
    //        newProperties.spineWidths[i].y = propertiesFirst.spineWidths[i].y * heredityRatio + propertiesSecond.spineWidths[i].y * (1 - heredityRatio);
    //    }

    //    return newProperties;
    //}

    void SetReady(bool success, Transform dinosaur)
    {
        if (success)
        {
            if (dinosaur == dinosaurFirst)
                dinosaurFirstReady = true;
            else if (dinosaur == dinosaurSecond)
                dinosaurSecondReady = true;
        }
    }

    public Transform DinosaurFirst
    {
        get { return dinosaurFirst; }
        set { dinosaurFirst = value; }
    }

    public Transform DinosaurSecond
    {
        get { return dinosaurSecond; }
        set { dinosaurSecond = value; }
    }
}
