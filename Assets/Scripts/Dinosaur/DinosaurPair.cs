using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurPair : MonoBehaviour
{
    [SerializeField] Transform dinosaurTransformFirst;
    [SerializeField] Transform dinosaurTransformSecond;

    [SerializeField] bool dinosaurFirstReady = false;
    [SerializeField] bool dinosaurSecondReady = false;

    const int boneCount = 23;
    const int legBoneCount = 3;

    Rigidbody rbFirst;
    Rigidbody rbSecond;

    Dinosaur dinosaurFirst;
    Dinosaur dinosaurSecond;

    DinosaurSpawner dinosaurSpawner;

    private void Start()
    {
        rbFirst = dinosaurTransformFirst.GetComponent<Rigidbody>();
        rbSecond = dinosaurTransformSecond.GetComponent<Rigidbody>();

        dinosaurFirst = dinosaurTransformFirst.GetComponent<DinosaurSetup>().Dinosaur;
        dinosaurSecond = dinosaurTransformSecond.GetComponent<DinosaurSetup>().Dinosaur;

        dinosaurSpawner = GetComponent<DinosaurSpawner>();

        StartRoute();
        StartCoroutine(CheckBothReady());
    }

    void StartRoute()
    {
        Unit unitInstanceFist = dinosaurTransformFirst.GetComponent<Unit>();
        Unit unitInstanceSecond = dinosaurTransformSecond.GetComponent<Unit>();

        unitInstanceFist.SetTarget(dinosaurTransformSecond, SetReady);
        unitInstanceSecond.SetTarget(dinosaurTransformFirst, SetReady);
    }

    IEnumerator CheckBothReady()
    {
        while (true)
        {
            rbFirst.velocity = rbSecond.velocity = Vector3.zero;

            if (dinosaurFirstReady && dinosaurSecondReady)
                CreateNewDinosaur();

            yield return new WaitForSeconds(0.5f);
        }
    }

    void CreateNewDinosaur()
    {
        Dinosaur newDinosaur = DetermineDinosaurProperties();
        Vector3 spawnPos = (dinosaurTransformFirst.position + dinosaurTransformSecond.position) / 2f;

        dinosaurSpawner.SpawnDinosaur(newDinosaur, spawnPos);

        dinosaurTransformFirst.GetComponent<DinosaurManager>().FinishPairing();
        dinosaurTransformSecond.GetComponent<DinosaurManager>().FinishPairing();

        Destroy(this);
    }

    Dinosaur DetermineDinosaurProperties()
    {
        float heredityRatio;

        // determine bends and widths along spine
        float[] spineBends = new float[boneCount];
        Vector2[] spineWidths = new Vector2[boneCount];

        for (int i = 0; i < boneCount; i++)
        {
            spineBends[i] = DetermineHeredity(dinosaurFirst.spineBends[i], dinosaurSecond.spineBends[i]);
            spineWidths[i].x = DetermineHeredity(dinosaurFirst.spineWidths[i].x, dinosaurSecond.spineWidths[i].x);
            spineWidths[i].y = DetermineHeredity(dinosaurFirst.spineWidths[i].y, dinosaurSecond.spineWidths[i].y);
        }

        // determine widths along leg bones
        Vector2[] legWidths = new Vector2[legBoneCount];

        for (int i = 0; i < legBoneCount; i++)
        {
            legWidths[i].x = DetermineHeredity(dinosaurFirst.legWidths[i].x, dinosaurSecond.legWidths[i].x);
            legWidths[i].y = DetermineHeredity(dinosaurFirst.legWidths[i].y, dinosaurSecond.legWidths[i].y);
        }

        // determine tail length
        int tailLength = Mathf.RoundToInt(DetermineHeredity(dinosaurFirst.tailLength, dinosaurSecond.tailLength));

        // determine neck length
        int neckLength = Mathf.RoundToInt(DetermineHeredity(dinosaurFirst.neckLength, dinosaurSecond.neckLength));

        // determine body and collider size
        heredityRatio = Random.Range(0.25f, 0.75f);
        float bodySize = DetermineHeredity(dinosaurFirst.bodySize, dinosaurSecond.bodySize, heredityRatio);
        Vector2 colliderSize = dinosaurFirst.colliderSize * heredityRatio + dinosaurSecond.colliderSize * (1 - heredityRatio);

        /*
            determine:
            * leg pair count (bipedal or not),
            * at whitch bone along the spine legs are attached to the body,
            * and the relative (to body) sizes of each leg pair
        */
        bool bipedal;
        List<int> legBoneIndices = new List<int>();
        List<float> legPairSizeRatios = new List<float>();

        int legPairCount;
        float legPairSizeRatio;

        if (dinosaurFirst.bipedal == dinosaurSecond.bipedal)
        {
            legPairCount = dinosaurFirst.legBoneIndices.Count;
            bipedal = dinosaurFirst.bipedal;

            for (int i = 0; i < legPairCount; i++)
            {
                legPairSizeRatio = DetermineHeredity(dinosaurFirst.legPairSizeRatios[i], dinosaurSecond.legPairSizeRatios[i]);
                legPairSizeRatios.Add(legPairSizeRatio);
            }

            legBoneIndices = (Random.value > 0.5f) ? dinosaurFirst.legBoneIndices : dinosaurSecond.legBoneIndices;
        }
        else
        {
            legPairSizeRatio = DetermineHeredity(dinosaurFirst.legPairSizeRatios[0], dinosaurSecond.legPairSizeRatios[0]);
            legPairSizeRatios.Add(legPairSizeRatio);

            Dinosaur fourLeggedDinosaur = !dinosaurFirst.bipedal ? dinosaurFirst : dinosaurSecond;
            Dinosaur twoLeggedDinosaur = dinosaurFirst.bipedal ? dinosaurFirst : dinosaurSecond;

            if (Random.value > 0.5f) // 50/50 chance that 2 and 4 - legged dinosaurs combo will result in either
            {
                bipedal = false;

                legPairSizeRatios.Add(fourLeggedDinosaur.legPairSizeRatios[1]);
                legBoneIndices = fourLeggedDinosaur.legBoneIndices;
            }
            else
            {
                bipedal = true;
                legBoneIndices = twoLeggedDinosaur.legBoneIndices;
            }    
        }

        // determine skin color
        heredityRatio = Random.Range(0.25f, 0.75f);
        Color skinColor = dinosaurFirst.skinColor * heredityRatio + dinosaurSecond.skinColor * (1 - heredityRatio);

        // determine walk properties
        heredityRatio = Random.Range(0.25f, 0.75f);
        DinosaurWalkingProperties walkProperties = new DinosaurWalkingProperties(
            DetermineHeredity(dinosaurFirst.walkingProperties.stepSpeed, dinosaurSecond.walkingProperties.stepSpeed),
            DetermineHeredity(dinosaurFirst.walkingProperties.stepDistance, dinosaurSecond.walkingProperties.stepDistance, heredityRatio),
            DetermineHeredity(dinosaurFirst.walkingProperties.stepLength, dinosaurSecond.walkingProperties.stepLength, heredityRatio),
            DetermineHeredity(dinosaurFirst.walkingProperties.stepHeight, dinosaurSecond.walkingProperties.stepHeight),
            DetermineHeredity(dinosaurFirst.walkingProperties.bodyBobAmount, dinosaurSecond.walkingProperties.bodyBobAmount)
            );

        Dinosaur newDinosaur = ScriptableObject.CreateInstance<Dinosaur>();
        newDinosaur.Init(
            bipedal,
            legBoneIndices,
            legPairSizeRatios,
            colliderSize,
            bodySize,
            tailLength,
            neckLength,
            spineBends,
            spineWidths,
            legWidths,
            skinColor,
            walkProperties
            );

         return newDinosaur;
    }

    static float DetermineHeredity(float firstValue, float secondValue, float ratio = -1)
    {
        float heredityRatio = (ratio == -1) ? Random.Range(0.25f, 0.75f) : ratio;
        return firstValue * heredityRatio + secondValue * (1 - heredityRatio);
    }

    void SetReady(bool success, Transform dinosaur)
    {
        if (success)
        {
            if (dinosaur == dinosaurTransformFirst)
                dinosaurFirstReady = true;
            else if (dinosaur == dinosaurTransformSecond)
                dinosaurSecondReady = true;
        }
    }

    public Transform DinosaurFirst
    {
        get { return dinosaurTransformFirst; }
        set { dinosaurTransformFirst = value; }
    }

    public Transform DinosaurSecond
    {
        get { return dinosaurTransformSecond; }
        set { dinosaurTransformSecond = value; }
    }
}