using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.GraphicsBuffer;

public class LegPrep : MonoBehaviour
{

    public Dinosaur dinosaur;
    public Transform rig;
    public Transform bodyRoot;

    MeshGen meshGen;
    int boneCount;
    GameObject tipBone;
    Transform legRoot;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.legWidths.Length;
    }

    public void PrepareLeg()
    {
        legRoot = transform.Find("Root");

        AdjustLegPairSize();

        for (int i = 0; i < boneCount; i++)
        {
            StretchLeg(i);
        }

        AddTipBone();
        SetUpIK();
    }

    void AdjustLegPairSize()
    {
        transform.parent.localScale = Vector3.one * 0.2f;
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
        GameObject legIKGO = new GameObject("2BoneIK_" + gameObject.name);
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
            dinosaur.walkingProperties.stepDistance,
            dinosaur.walkingProperties.stepLength,
            dinosaur.walkingProperties.stepHeight,
            dinosaur.walkingProperties.footOffset,
            dinosaur.walkingProperties.bodyBobAmount
            );
    }
}
