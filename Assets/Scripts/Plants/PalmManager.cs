using Dreamteck.Splines;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEditor.Progress;

public class PalmManager : MonoBehaviour
{

    [SerializeField] GameObject trunkSegment;
    [SerializeField] GameObject leaf;
    [SerializeField] GameObject coconut;

    [SerializeField] int segmentCount = 14;
    [SerializeField] float maxSegmentAngle = 12.5f;
    [SerializeField] float subsequentSegmentScaleRatio = 0.95f;
    [SerializeField] float upwardLeanRatio = 0.925f;
    [SerializeField] float trunkCurviness = 175f;

    [SerializeField] float leafDensity = 12;
    [SerializeField] float leafRandomness = 0.3f;

    [SerializeField] AnimationCurve popCurve;
    [SerializeField] float growSpeed = 2f;

    [SerializeField] bool beingWatered;

    VegetationManager vegetationManager;

    float segmentPopDuration = 0.4f;

    Transform[] trunkSegments;
    List<Transform> leafInstances;
    [NonSerialized]
    public List<Transform> coconutInstances;

    Transform trunk;
    Transform leaves;
    Transform coconuts;
    float maxLeafAngleOffset = 10f;
    int visibleSegmentCount = 1;


    private void Awake()
    {
        vegetationManager = transform.parent.GetComponent<VegetationManager>();

        trunk = transform.Find("Trunk");
        leaves = transform.Find("Leaves");
        coconuts = transform.Find("Coconuts");

        trunkSegments = new Transform[segmentCount];
        leafInstances = new List<Transform>();
        coconutInstances = new List<Transform>();
    }

    private void Start()
    {
        GenerateTrunk();
        GenerateLeaves();

        StartCoroutine(HandleGrowInteraction());
    }

    IEnumerator HandleGrowInteraction()
    {
        int segmentIndex = 0;

        while (true)
        {
            if (visibleSegmentCount == segmentCount)
            {
                SpawnCoconuts();
                CreateCoconutTree();
                yield break;
            }
                
            if (beingWatered || vegetationManager.plantsAutoGrow)
            {
                if (segmentIndex >= visibleSegmentCount)
                {
                    trunkSegments[visibleSegmentCount].gameObject.SetActive(true);
                    visibleSegmentCount++;

                    UpdateLeaves(visibleSegmentCount);

                    segmentIndex = 0;
                }
                else
                {
                    if (segmentIndex < visibleSegmentCount - 1)
                        StartCoroutine(PopItem(trunkSegments[segmentIndex]));

                    segmentIndex++;
                }
            }

            yield return new WaitForSeconds(0.2f / growSpeed);
        }
    }

    void UpdateLeaves(int currentSegmentIndex)
    {
        leaves.rotation = trunkSegments[visibleSegmentCount - 1].rotation;
        leaves.position = trunkSegments[visibleSegmentCount - 1].position + leaves.up * 0.5f;

        float leafSize = 0.5f + 0.5f * ((float)currentSegmentIndex / segmentCount);
        leaves.localScale = Vector3.one * leafSize;
        StartCoroutine(PopItem(leaves));
    }

    IEnumerator PopItem(Transform item)
    {
        float startTime = Time.time;
        Vector3 startScale = item.localScale;
        Vector3 peakScale = startScale * 2f;

        while (true)
        {
            float elapsedTime = Time.time - startTime;
            float popStage = (elapsedTime * growSpeed) / segmentPopDuration;
            item.localScale = Vector3.Lerp(startScale, peakScale, popCurve.Evaluate(popStage));

            if (popStage > 1)
            {
                item.localScale = startScale;
                yield break;
            }
            else yield return null;
        }
    }

    void GenerateTrunk()
    {
        Vector3 segmentPosRelative = Vector3.up * 0.014f;
        Vector3 segmentScale = Vector3.one;

        for (int i = 0; i < segmentCount; i++)
        {
            trunkSegments[i] = Instantiate(trunkSegment, trunk).transform;

            if (i == 0)
                trunkSegments[i].localPosition = Vector3.zero;
            else
                trunkSegments[i].position = trunkSegments[i - 1].TransformPoint(segmentPosRelative);

            trunkSegments[i].localScale = segmentScale;
            trunkSegments[i].rotation = CalculateSegmentRotation(i);

            segmentScale *= subsequentSegmentScaleRatio;

            if (i > 0) trunkSegments[i].gameObject.SetActive(false);
        }
    }

    void GenerateLeaves()
    {
        Transform trunkTop = trunkSegments[0];

        leaves.rotation = trunkTop.rotation;
        leaves.position = trunkTop.position + leaves.up * 0.5f;

        float angleOffset = UnityEngine.Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
        float angleInterval = 360 / leafDensity;

        for (float angle = angleOffset; angle < 360f; angle += angleOffset + angleInterval)
        {
            leafInstances.Add(SetUpLeaf(angle));

            angleOffset = UnityEngine.Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
        }

        leaves.localScale = Vector3.one * 0.5f;
    }

    Quaternion CalculateSegmentRotation(int segmentIndex)
    {
        Quaternion rotationOffset = Quaternion.Euler(RandomVector(maxSegmentAngle));
        if (segmentIndex == 0) return rotationOffset;

        Quaternion rotation = trunkSegments[segmentIndex - 1].transform.rotation * rotationOffset;
        float segmentUpwardLean = Mathf.Pow(upwardLeanRatio, segmentIndex) * trunkCurviness;
        return Quaternion.Euler(new Vector3(rotation.x, rotation.y, rotation.z) * segmentUpwardLean);
    }

    Transform SetUpLeaf(float angle)
    {
        Transform leafInstance = Instantiate(leaf, leaves).transform;
        Vector3 rotationVector = new Vector3(UnityEngine.Random.Range(-15, 15), angle, UnityEngine.Random.Range(-30, 0));
        leafInstance.localRotation = Quaternion.Euler(rotationVector);

        SplineComputer leafSC = leafInstance.GetComponent<SplineComputer>();
        SplinePoint[] splinePoints = leafSC.GetPoints();

        for (int i = 1; i < splinePoints.Length; i++)
        {
            splinePoints[i].position += RandomVector(leafRandomness);
        }
        leafSC.SetPoints(splinePoints);

        foreach (Transform splinePoint in leafInstance)
        {
            Destroy(splinePoint.gameObject);
        }

        return leafInstance;
    }

    void SpawnCoconuts()
    {
        int coconutCount = UnityEngine.Random.Range(1, 4);
        Transform trunkTop = trunkSegments[segmentCount - 1];

        for (int i = 0; i < coconutCount; i++)
        {
            Transform coconutInstance = Instantiate(coconut, coconuts).transform;
            Vector2 coconutPosOffset = UnityEngine.Random.insideUnitCircle.normalized * 0.01f;
            coconutInstance.position = trunkTop.TransformPoint(new Vector3(coconutPosOffset.x, 0, coconutPosOffset.y));

            PopItem(coconutInstance);

            coconutInstances.Add(coconutInstance);
        }
    }

    void CreateCoconutTree()
    {
        CoconutTree coconutTree = new CoconutTree(transform, coconutInstances);
        vegetationManager.AddCoconutTree(coconutTree);
    }

    static Vector3 RandomVector(float offset)
    {
        return new Vector3(UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset), UnityEngine.Random.Range(-offset, offset));

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "WateringCan")
            beingWatered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WateringCan")
            beingWatered = false;
    }

}
