using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PalmManager : MonoBehaviour
{

    [SerializeField] GameObject trunkSegment;
    [SerializeField] GameObject leaf;

    [SerializeField] int segmentCount = 14;
    [SerializeField] float maxSegmentAngle = 12.5f;
    [SerializeField] float subsequentSegmentScaleRatio = 0.95f;
    [SerializeField] float upwardLeanRatio = 0.925f;
    [SerializeField] float trunkCurviness = 175f;

    [SerializeField] float leafDensity = 12;
    [SerializeField] float leafRandomness = 0.3f;

    [SerializeField] AnimationCurve popCurve;
    [SerializeField] float segmentPopDuration = 0.4f;

    Transform[] trunkSegments;
    List<Transform> leafInstances;

    Transform trunk;
    Transform leaves;
    float maxLeafAngleOffset = 10f;
    int visibleSegmentCount = 1;

    int palmTreelayer;
    LayerMask palmTreelayerMask;


    private void Awake()
    {
        trunk = transform.Find("Trunk");
        leaves = transform.Find("Leaves");

        trunkSegments = new Transform[segmentCount];
        leafInstances = new List<Transform>();

        palmTreelayer = LayerMask.NameToLayer("PalmTree");
        palmTreelayerMask |= (1 << palmTreelayer);
    }

    private void Start()
    {
        GenerateTrunk();
        GenerateLeaves();

        StartCoroutine(HandleGrowInterraction());
    }

    IEnumerator HandleGrowInterraction()
    {
        int segmentIndex = 0;

        while (true)
        {
            if (visibleSegmentCount == segmentCount) yield break;

            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine(PopSegment(trunkSegments[segmentIndex]));

                if (segmentIndex >= visibleSegmentCount)
                {
                    trunkSegments[visibleSegmentCount].gameObject.SetActive(true);
                    visibleSegmentCount++;

                    UpdateLeaves(visibleSegmentCount);

                    segmentIndex = 0;
                }
                else segmentIndex++;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void UpdateLeaves(int currentSegmentIndex)
    {
        leaves.rotation = trunkSegments[visibleSegmentCount - 1].rotation;
        leaves.position = trunkSegments[visibleSegmentCount - 1].position + leaves.up * 0.5f;

        float leafSize = 0.5f + 0.5f * ((float)currentSegmentIndex / segmentCount);
        leaves.localScale = Vector3.one * leafSize;
        StartCoroutine(PopSegment(leaves));
    }

    IEnumerator PopSegment(Transform segment)
    {
        float startTime = Time.time;
        Vector3 startScale = segment.localScale;
        Vector3 peakScale = startScale * 1.5f;

        while (true)
        {
            float popStage = (Time.time - startTime) / segmentPopDuration;
            segment.localScale = Vector3.Lerp(startScale, peakScale, popCurve.Evaluate(popStage));

            if (popStage > 1)
            {
                segment.localScale = startScale;
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
            trunkSegments[i].gameObject.layer = palmTreelayer;

            trunkSegments[i].AddComponent<MeshCollider>();

            segmentScale *= subsequentSegmentScaleRatio;

            if (i > 0) trunkSegments[i].gameObject.SetActive(false);
        }
    }

    void GenerateLeaves()
    {
        Transform trunkTop = trunkSegments[0];

        leaves.rotation = trunkTop.rotation;
        leaves.position = trunkTop.position + leaves.up * 0.5f;

        float angleOffset = Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
        float angleInterval = 360 / leafDensity;

        for (float angle = angleOffset; angle < 360f; angle += angleOffset + angleInterval)
        {
            leafInstances.Add(SetUpLeaf(angle));

            angleOffset = Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
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
        Vector3 rotationVector = new Vector3(Random.Range(-15, 15), angle, Random.Range(-30, 0));
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

    static Vector3 RandomVector(float offset)
    {
        return new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));

    }

}
