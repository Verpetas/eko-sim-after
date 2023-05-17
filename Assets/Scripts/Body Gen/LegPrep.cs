using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LegPrep : MonoBehaviour
{

    [SerializeField] Transform rig;
    [SerializeField] Transform bodyRoot;

    Dinosaur dinosaur;
    MeshGen meshGen;
    int boneCount;
    GameObject tipBone;
    Transform legRoot;
    float legPairSize;
    float legPairSizeRelative;

    private void Awake()
    {
        dinosaur = bodyRoot.parent.parent.GetComponent<DinosaurSetup>().Dinosaur;

        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.legWidths.Length;
    }

    public void PrepareLeg()
    {
        legRoot = transform.Find("Root");

        legPairSize = GetLegPairSize();
        legPairSizeRelative = GetLegPairSizeRelative(legPairSize);
        transform.parent.localScale = Vector3.one * legPairSize;

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

        return dinosaur.legPairSizes[legPairIndex];
    }

    float GetLegPairSizeRelative(float currentPairSize)
    {
        float legPairSizeRelative = 1f;

        for (int i = 0; i < dinosaur.legPairSizes.Count; i++)
        {
            if (dinosaur.legPairSizes[i] > currentPairSize)
                legPairSizeRelative = currentPairSize / dinosaur.legPairSizes[i];
        }

        return legPairSizeRelative;
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

        GameObject target = new GameObject("Target");
        target.transform.SetParent(legIKGO.transform);

        GameObject hint = new GameObject("Hint");
        hint.transform.SetParent(legIKGO.transform);

        AssignLegIKConstraint(target, hint);
        AssignIKLegSolver(target);
    }

    void AssignLegIKConstraint(GameObject target, GameObject hint)
    {
        FastIKFabric legIK = tipBone.AddComponent<FastIKFabric>();
        Destroy(legIK.Target.gameObject); // removes redundant target
        legIK.Target = target.transform;
        legIK.Pole = hint.transform;
    }

    void AssignIKLegSolver(GameObject target)
    {
        IKFootSolver legIKSolver = target.AddComponent<IKFootSolver>();
        legIKSolver.legRoot = legRoot;
        legIKSolver.bodyRoot = bodyRoot;
        AssignWalkProperties(legIKSolver);
    }

    void AssignWalkProperties(IKFootSolver legIKSolver)
    {
        legIKSolver.AssignWalkProperties(
            dinosaur.walkingProperties.stepSpeed,
            dinosaur.walkingProperties.stepDistance * dinosaur.bodySize * legPairSizeRelative,
            dinosaur.walkingProperties.stepLength * dinosaur.bodySize * legPairSizeRelative,
            dinosaur.walkingProperties.stepHeight * dinosaur.bodySize * legPairSizeRelative,
            dinosaur.walkingProperties.footOffset,
            dinosaur.walkingProperties.bodyBobAmount * dinosaur.bodySize
            );
    }

}
