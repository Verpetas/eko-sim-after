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
    RigBuilder rigBuilder;
    Transform legRoot;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        rigBuilder = bodyRoot.GetComponent<RigBuilder>();
        boneCount = dinosaur.legWidths.Length;
        legRoot = transform.Find("Root");
    }

    public void PrepareLeg()
    {
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

        AssignLegIKConstraint(legIKGO, target, hint);
        AssignIKLegSolver(target);

        rigBuilder.Build();
    }

    void AssignLegIKConstraint(GameObject legIKGO, GameObject target, GameObject hint)
    {
        TwoBoneIKConstraint legIK = legIKGO.AddComponent<TwoBoneIKConstraint>();
        legIK.Reset();
        legIK.data.root = meshGen.boneTransforms[0];
        legIK.data.mid = meshGen.boneTransforms[1];
        legIK.data.tip = tipBone.transform;
        legIK.data.target = target.transform;
        legIK.data.hint = hint.transform;
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
