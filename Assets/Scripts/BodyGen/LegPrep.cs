using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LegPrep : MonoBehaviour
{

    public Dinosaur dinosaur;
    public Transform rig;

    MeshGen meshGen;
    int boneCount;
    GameObject tipBone;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.legWidths.Length;
    }

    public void PrepareLeg()
    {

        for (int i = 0; i < boneCount; i++)
        {
            StretchLeg(i);
        }

        AddTipBone();
        SetUpIK();
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
        tipBone.transform.localPosition = new Vector3(0, 0, 200);
    }

    void SetUpIK()
    {
        GameObject legIKGO = new GameObject("2BoneIK_" + gameObject.name);
        legIKGO.transform.SetParent(rig);

        GameObject target = new GameObject("Target");
        target.transform.SetParent(legIKGO.transform);

        TwoBoneIKConstraint legIK = legIKGO.AddComponent<TwoBoneIKConstraint>();
        legIK.data.root = meshGen.boneTransforms[0];
        legIK.data.mid = meshGen.boneTransforms[1];
        legIK.data.tip = tipBone.transform;
        legIK.data.target = target.transform;

        IKFootSolver legIKSolver = target.AddComponent<IKFootSolver>();
        // to assign some properties here (that are not in scriptoable object)
        AssignWalkProperties(legIKSolver);
    }

    void AssignWalkProperties(IKFootSolver legIKSolver)
    {
        // to pass properties through public class
        //legIKSolver.
    }
}
