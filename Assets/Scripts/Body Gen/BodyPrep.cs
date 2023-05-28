using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.LowLevel;

public class BodyPrep : MonoBehaviour
{

    [SerializeField] Transform rig;
    [SerializeField] Transform wrapper;
    [SerializeField] float tailStiffness = 75f;
    [SerializeField] float tailBounciness = 75f;
    [SerializeField] float tailDampness = 0.2f;

    MeshGen meshGen;
    int boneCount;
    float[] spineBendsGlobal;
    Transform root;
    List<Transform> legBones = new List<Transform>();
    LayerMask dinosaurLayerMask;
    RigBuilder rigBuilder;
    int dinosaurLayer;
    GameObject colliderGO;

    DinosaurSetup dinosaurSetup;
    DinosaurManager dinosaurManager;
    Dinosaur dinosaur;

    ChainIKConstraint neckIK;

    SpringBone[] tailSprings;

    private void Awake()
    {
        dinosaurSetup = wrapper.parent.GetComponent<DinosaurSetup>();
        dinosaurManager = wrapper.parent.GetComponent<DinosaurManager>();
        dinosaur = dinosaurSetup.Dinosaur;

        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.spineBends.Length;

        root = transform.parent;

        spineBendsGlobal = new float[boneCount];
        CalculateGlobalSpineBends();

        dinosaurLayer = LayerMask.NameToLayer("Dinosaur");
        dinosaurLayerMask |= (1 << dinosaurLayer);

        tailSprings = new SpringBone[dinosaur.tailLength];
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
        meshGen.skinnedMeshRenderer.enabled = false;

        AdjustBodySize();

        for (int i = 0; i < boneCount; i++)
        {
            BendBody(i);
            StretchBody(i);
        }

        RestructureBones();

        SetUpRig();

        AddTailPhysics();
        AddNeckIK();

        FindLegBones();
        CenterBody();

        CreateTempCollider();

        AttachLegs();
        SwapColliders();

        PrepareDinosaur();
    }

    void AdjustBodySize()
    {
        transform.localScale = Vector3.one * dinosaur.bodySize;
    }

    void BendBody(int boneIndex)
    {
        meshGen.boneTransforms[boneIndex].rotation = Quaternion.Euler(spineBendsGlobal[boneIndex], 0, 0);
    }

    void StretchBody(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, dinosaur.spineWidths[boneIndex].x);
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, dinosaur.spineWidths[boneIndex].y);
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

    void SetUpRig()
    {
        Rig rigComp = rig.AddComponent<Rig>();
        RigLayer rigLayer = new RigLayer(rigComp, true);

        root.AddComponent<Animator>();
        rigBuilder = root.AddComponent<RigBuilder>();

        rigBuilder.layers.Add(rigLayer);
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

        Vector3 legBoneRelativePos = legBones[0].position - firstBone.position;

        if (!dinosaur.bipedal)
        {
            Vector3 secondRelPos = legBones[1].position - firstBone.position;
            legBoneRelativePos = (legBoneRelativePos + secondRelPos) / 2f;
        }

        wrapper.localPosition = -legBoneRelativePos;
    }

    void CreateTempCollider()
    {
        Mesh bakedMesh = new Mesh();
        meshGen.skinnedMeshRenderer.BakeMesh(bakedMesh);

        Transform dinosaurModel = transform.Find("Model");

        colliderGO = new GameObject("Collider");
        colliderGO.transform.parent = dinosaurModel;
        colliderGO.transform.localPosition = Vector3.zero;

        colliderGO.layer = dinosaurLayer;

        MeshCollider meshCollider = colliderGO.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = bakedMesh;
    }

    void AttachLegs()
    {
        for (int i = 0; i < legBones.Count; i++)
        {
            Transform legPair = root.Find("LegPair_" + i);
            Vector3 legPairPosOffset = Vector3.up * 10f * dinosaur.bodySize;
            legPair.position = legBones[i].position + legPairPosOffset;

            Vector3 rayStart = legPair.TransformPoint(Vector3.left * 300f);
            Vector3 rayDir = legPair.TransformPoint(Vector3.zero) - rayStart;

            Ray ray = new Ray(rayStart, rayDir);
            if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, dinosaurLayerMask))
            {
                Transform legL = legPair.Find("Leg_L");
                Transform legR = legPair.Find("Leg_R");
                legL.position = info.point;
                //legL.localPosition *= 1.2f;
                legR.localPosition = new Vector3(-legL.localPosition.x, 0, 0);
            }
        }
    }

    void AddTailPhysics()
    {
        for (int i = 0; i < dinosaur.tailLength; i++)
        {
            tailSprings[i] = meshGen.boneTransforms[i].AddComponent<SpringBone>();

            tailSprings[i].useSpecifiedRotation = true;
            tailSprings[i].customRotation = meshGen.boneTransforms[i].localRotation.eulerAngles;
            tailSprings[i].stiffness = tailStiffness;
            tailSprings[i].bounciness = tailBounciness;
            tailSprings[i].dampness = tailDampness;
            tailSprings[i].springEnd = meshGen.boneTransforms[i].localRotation * -Vector3.forward * 50f;
        }

        UpdateTail(1f);
    }

    void AddNeckIK()
    {
        GameObject neckIKGO = new GameObject("ChainIK_Neck");
        neckIKGO.transform.parent = rig;

        GameObject headTarget = new GameObject("Target");
        headTarget.transform.parent = neckIKGO.transform;
        headTarget.transform.position = meshGen.boneTransforms[boneCount - 1].position;

        neckIK = neckIKGO.AddComponent<ChainIKConstraint>();

        neckIK.Reset();
        neckIK.weight = 0;
        neckIK.data.tip = meshGen.boneTransforms[boneCount - 1];
        neckIK.data.root = meshGen.boneTransforms[boneCount - dinosaur.neckLength - 1];
        neckIK.data.chainRotationWeight = 1;
        neckIK.data.tipRotationWeight = 0;
        neckIK.data.target = headTarget.transform;

        rigBuilder.Build();
    }

    void SwapColliders()
    {
        DestroyImmediate(colliderGO);
        dinosaurSetup.AddCollider(
            dinosaur.colliderSize.x * dinosaur.bodySize,
            dinosaur.colliderSize.y * dinosaur.bodySize);
    }

    void PrepareDinosaur()
    {
        dinosaurSetup.AssignSpawnPosition();
        
        dinosaurManager.enabled = true;
        dinosaurManager.NeckIK = neckIK;
        dinosaurManager.Body = this;

        dinosaurManager.AddRB();
        dinosaurManager.EnablePathfinding();
    }

    public void UpdateTail(float dinosaurGrowth)
    {
        foreach (SpringBone tailSpring in tailSprings)
        {
            tailSpring.BodySize = dinosaurGrowth * dinosaur.bodySize;
        }

        rigBuilder.Build();
    }

    public SkinnedMeshRenderer MeshRenderer
    {
        get { return meshGen.skinnedMeshRenderer; }
    }

}
