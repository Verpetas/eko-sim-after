using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LegPrep : MonoBehaviour
{

    [SerializeField] Transform rig;
    [SerializeField] Transform bodyRoot;
    [SerializeField] Material legSkin;

    Dinosaur dinosaur;
    MeshGen meshGen;
    int boneCount;
    GameObject tipBone;
    Transform legRoot;
    float legPairSizeRelative;

    GameObject target;
    GameObject hint;
    FastIKFabric legIK;
    IKFootSolver legIKSolver;

    private void Awake()
    {
        Transform topMost = bodyRoot.parent.parent;
        dinosaur = topMost.GetComponent<DinosaurSetup>().Dinosaur;

        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.legWidths.Length;
    }

    public void PrepareLeg()
    {
        meshGen.skinnedMeshRenderer.enabled = false;

        legRoot = transform.Find("Root");

        float legPairSize = GetLegPairSize();
        legPairSizeRelative = GetLegPairSizeRelative(legPairSize);
        transform.parent.localScale = Vector3.one * legPairSize;

        AssignSkin();

        for (int i = 0; i < boneCount; i++)
        {
            StretchLeg(i);
        }

        AddTipBone();
        SetUpIK();
    }

    float GetLegPairSize()
    {
        string legPairName = transform.parent.name;
        int legPairIndex = legPairName[legPairName.Length - 1] - '0';

        return dinosaur.legPairSizeRatios[legPairIndex] * dinosaur.bodySize;
    }

    float GetLegPairSizeRelative(float currentPairSize)
    {
        float legPairSizeRelative = 1f;

        for (int i = 0; i < dinosaur.legPairSizeRatios.Count; i++)
        {
            if (dinosaur.legPairSizeRatios[i] > currentPairSize)
                legPairSizeRelative = currentPairSize / dinosaur.legPairSizeRatios[i];
        }

        return legPairSizeRelative;
    }

    void AssignSkin()
    {
        meshGen.skinnedMeshRenderer.material = legSkin;
        meshGen.skinnedMeshRenderer.material.color = dinosaur.skinColor;
    }

    void StretchLeg(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, dinosaur.legWidths[boneIndex].x);
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, dinosaur.legWidths[boneIndex].y);
    }

    void AddTipBone()
    {
        tipBone = new GameObject("Bone_1_end");
        tipBone.transform.SetParent(meshGen.boneTransforms[meshGen.boneTransforms.Length - 1]);
        tipBone.transform.localPosition = new Vector3(0, 0, 200f);
    }

    void SetUpIK()
    {
        string legPairGOName = transform.parent.gameObject.name;
        char legPairNo = legPairGOName[legPairGOName.Length - 1];

        GameObject legIKGO = new GameObject("2BoneIK_" + gameObject.name + "_" + legPairNo);
        legIKGO.transform.SetParent(rig);

        target = new GameObject("Target");
        target.transform.SetParent(legIKGO.transform);

        hint = new GameObject("Hint");
        hint.transform.SetParent(legIKGO.transform);

        AssignLegIKConstraint();
        AssignIKLegSolver();
        AssignWalkProperties(1f);
    }

    void AssignLegIKConstraint()
    {
        legIK = tipBone.AddComponent<FastIKFabric>();
        Destroy(legIK.Target.gameObject); // removes redundant target
        legIK.Target = target.transform;
        legIK.Pole = hint.transform;
    }

    void AssignIKLegSolver()
    {
        legIKSolver = target.AddComponent<IKFootSolver>();
        legIKSolver.legRoot = legRoot;
        legIKSolver.bodyRoot = bodyRoot;
    }

    void AssignWalkProperties(float growth)
    {
        legIKSolver.AssignWalkProperties(
            dinosaur.walkingProperties.stepSpeed,
            dinosaur.walkingProperties.stepDistance * dinosaur.bodySize * legPairSizeRelative * growth,
            dinosaur.walkingProperties.stepLength * dinosaur.bodySize * legPairSizeRelative * growth,
            dinosaur.walkingProperties.stepHeight * dinosaur.bodySize * legPairSizeRelative * growth,
            dinosaur.walkingProperties.bodyBobAmount * dinosaur.bodySize
            );
    }

    public void UpdateLegs(float dinosaurGrowth)
    {
        legIKSolver.enabled = false;
        legIK.enabled = false;

        meshGen.boneTransforms[0].localRotation = meshGen.boneTransforms[1].localRotation = tipBone.transform.localRotation = Quaternion.identity;

        legIK.Init();
        legIK.enabled = true;
        legIKSolver.enabled = true;

        AssignWalkProperties(dinosaurGrowth);
    }

    public SkinnedMeshRenderer MeshRenderer
    {
        get { return meshGen.skinnedMeshRenderer; }
    }

}
