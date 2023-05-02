using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPrep : MonoBehaviour
{

    public Dinosaur dinosaur;

    MeshGen meshGen;
    int boneCount;
    float[] spineBendsGlobal;
    Transform root;
    List<Transform> legBones = new List<Transform>();
    LayerMask dinosaurLayerMask;
    GameObject dinosaurModel;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.spineBends.Length;

        root = transform.parent;

        spineBendsGlobal = new float[boneCount];
        CalculateGlobalSpineBends();
    }

    void CalculateGlobalSpineBends()
    {
        spineBendsGlobal[0] = 360f + dinosaur.spineBends[0];

        for (int i = 1; i < boneCount; i++)
        {
            spineBendsGlobal[i] = spineBendsGlobal[i - 1] + dinosaur.spineBends[i];
        }
    }

    public void PrepareBody()
    {
        AssignLayer();

        for (int i = 0; i < boneCount; i++)
        {
            BendBody(i);
            StretchBody(i);
        }

        RestructureBones();
        FindLegBones();
        CenterBody();
        CreateTempCollider();
        AttachLegs();

        //AddRB();

        //AdjustRotation();
    }

    void AssignLayer()
    {
        int dinosaurLayer = LayerMask.NameToLayer("Dinosaur");
        dinosaurModel = transform.Find("Model").gameObject;
        dinosaurModel.layer = dinosaurLayer;
        dinosaurLayerMask |= (1 << dinosaurLayer);
    }

    void BendBody(int boneIndex)
    {
        meshGen.boneTransforms[boneIndex].rotation = Quaternion.Euler(spineBendsGlobal[boneIndex], 0, 0);
    }

    void StretchBody(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, dinosaur.spineWidthsX[boneIndex]);
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, dinosaur.spineWidthsY[boneIndex]);
    }

    void RestructureBones()
    {
        int tailLength = dinosaur.tailLength;

        meshGen.boneTransforms[tailLength - 1].parent = meshGen.root;
        meshGen.boneTransforms[tailLength].parent = meshGen.root;

        // structure tail bones
        for (int i = tailLength - 2; i >= 0; i--)
        {
            meshGen.boneTransforms[i].parent = meshGen.boneTransforms[i + 1];
        }

        // structure bones for the rest of the body
        for (int i = tailLength + 1; i < boneCount; i++)
        {
            meshGen.boneTransforms[i].parent = meshGen.boneTransforms[i - 1];
        }
    }

    void FindLegBones()
    {
        foreach (int index in dinosaur.legBoneIndices)
        {
            legBones.Add(meshGen.boneTransforms[index]);
        }
    }

    void CenterBody()
    {
        Transform firstBone = meshGen.boneTransforms[0];

        float distanceToLegs = Vector3.Distance(firstBone.position, legBones[0].position);

        if (!dinosaur.bipedal)
            distanceToLegs = (distanceToLegs + Vector3.Distance(firstBone.position, legBones[1].position)) / 2f;

        root.position += new Vector3(0, 0, -distanceToLegs);
    }

    void CreateTempCollider()
    {
        Mesh bakedMesh = new Mesh();
        meshGen.skinnedMeshRenderer.BakeMesh(bakedMesh);

        MeshCollider meshCollider = dinosaurModel.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
    }

    void AttachLegs()
    {
        for (int i = 0; i < legBones.Count; i++)
        {
            Transform legPair = root.Find("LegPair_" + i);
            legPair.position = legBones[i].position + Vector3.up * 10f;

            Vector3 rayStart = legPair.TransformPoint(Vector3.left * 300f);
            Vector3 rayDir = legPair.TransformPoint(Vector3.zero) - rayStart;

            Ray ray = new Ray(rayStart, rayDir);
            if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, dinosaurLayerMask))
            {
                Transform legL = legPair.Find("Leg_L");
                Transform legR = legPair.Find("Leg_R");
                legL.position = info.point;
                legR.localPosition = new Vector3(-legL.localPosition.x, 0, 0);
            }
        }
    }

    void AddRB()
    {

    }

    void AdjustRotation()
    {
        Quaternion rotStart = Quaternion.Euler(0, 0, 0);
        Quaternion rotEnd = meshGen.boneTransforms[boneCount - 1].rotation;
        meshGen.boneTransforms[0].rotation = Quaternion.Inverse(Quaternion.Slerp(rotStart, rotEnd, 0.5f));
    }

}
