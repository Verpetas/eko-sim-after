using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PalmManager : MonoBehaviour
{

    [SerializeField] GameObject trunkSegment;
    [SerializeField] GameObject leaf;

    [SerializeField] int segmentCount = 14;
    [SerializeField] float maxSegmentAngle = 12.5f;
    [SerializeField] float subsequentSegmentScaleRatio = 0.994f;
    [SerializeField] float upwardLeanRatio = 0.925f;
    [SerializeField] float trunkCurviness = 175f;

    [SerializeField] float leafDensity = 12;
    [SerializeField] float leafRandomness = 0.3f;

    Transform[] trunkSegments;
    List<Transform> leafInstances;

    Transform trunk;
    Transform leaves;
    float maxLeafAngleOffset = 10f;

    private void Awake()
    {
        trunk = transform.Find("Trunk");
        leaves = transform.Find("Leaves");

        trunkSegments = new Transform[segmentCount];
        leafInstances = new List<Transform>();
    }

    private void Start()
    {
        GenerateTrunk();
        GenerateLeaves();
    }

    void GenerateTrunk()
    {
        Transform parent = trunk;
        Vector3 segmentPos = Vector3.up * 0.014f;
        Vector3 segmentScale = Vector3.one;

        for (int i = 0; i < segmentCount; i++)
        {
            trunkSegments[i] = Instantiate(trunkSegment, parent).transform;
            trunkSegments[i].localPosition = segmentPos;
            trunkSegments[i].localScale = segmentScale;
            trunkSegments[i].rotation = CalculateSegmentRotation(i);

            segmentScale *= subsequentSegmentScaleRatio;
            parent = trunkSegments[i];
        }
    }

    void GenerateLeaves()
    {
        Transform trunkTop = trunkSegments[segmentCount - 1];

        leaves.rotation = trunkTop.rotation;
        leaves.position = trunkTop.position + leaves.up * 0.5f;

        float angleOffset = Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
        float angleInterval = 360 / leafDensity;

        for (float angle = angleOffset; angle < 360f; angle += angleOffset + angleInterval)
        {
            leafInstances.Add(SetUpLeaf(angle));

            angleOffset = Random.Range(-maxLeafAngleOffset, maxLeafAngleOffset);
        }

        leaves.parent = trunkTop;
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

        for (int i = 1; i < 4; i++)
        {
            Transform splinePoint = leafInstance.GetChild(i);
            splinePoint.position += RandomVector(leafRandomness);
        }

        return leafInstance;
    }

    static Vector3 RandomVector(float offset)
    {
        return new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));

    }

}
