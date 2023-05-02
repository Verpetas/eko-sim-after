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
    Transform legs;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.spineBends.Length;

        root = transform.parent;
        legs = root.Find("Legs");

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
        for (int i = 0; i < boneCount; i++)
        {
            BendBody(i);
            StretchBody(i);
        }

        RestructureBones();
        CenterBody();

        CreateTempCollider();

        AttachLegs();

        //AddRB();

        //AdjustRotation();
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

    void CenterBody()
    {
        //Transform firstBone = meshGen.boneTransforms[0];

        //int legPairCount = dinosaur.legBoneIndices.Count;
        //float distanceToLegsSum = 0;

        //for (int i = 0; i < legPairCount; i++)
        //{
        //    Transform legBone = meshGen.boneTransforms[dinosaur.legBoneIndices[i]];
        //    float distanceToLegs = Vector3.Distance(firstBone.position, legBone.position);
        //    distanceToLegsSum += distanceToLegs;
        //}

        //float zOffset = - distanceToLegsSum / legPairCount;
        //root.position += new Vector3(0, 0, zOffset);

        Transform firstBone = meshGen.boneTransforms[0];

        Transform legBone = meshGen.boneTransforms[dinosaur.legBoneIndices[0]];
        float distanceToLegs = Vector3.Distance(firstBone.position, legBone.position);

        if (!dinosaur.bipedal)
        {
            legBone = meshGen.boneTransforms[dinosaur.legBoneIndices[1]];
            distanceToLegs = (distanceToLegs + Vector3.Distance(firstBone.position, legBone.position)) / 2f;
        }

        root.position += new Vector3(0, 0, -distanceToLegs);
    }

    void CreateTempCollider()
    {
        GameObject dinosaurModel = transform.Find("Model").gameObject;

        Mesh bakedMesh = new Mesh();
        meshGen.skinnedMeshRenderer.BakeMesh(bakedMesh);

        MeshCollider meshCollider = dinosaurModel.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
    }

    void AttachLegs()
    {
        
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
