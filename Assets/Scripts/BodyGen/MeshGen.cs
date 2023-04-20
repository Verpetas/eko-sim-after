using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshGen : MonoBehaviour
{

    [SerializeField] int segments = 20;
    [SerializeField] float radius = 1;
    [SerializeField] int rings = 20;
    [SerializeField] float length = 15;
    [SerializeField] int boneCount = 10;

    [SerializeField] Material bodyMaterial;
    [SerializeField] bool legGen;

    Transform root;
    [NonSerialized] public SkinnedMeshRenderer skinnedMeshRenderer;
    [NonSerialized] public MeshCollider meshCollider;
    [NonSerialized] public Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<BoneWeight> boneWeights = new List<BoneWeight>();

    [NonSerialized] public Transform[] boneTransforms;
    Matrix4x4[] bindPoses;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        boneTransforms = new Transform[boneCount];
        bindPoses = new Matrix4x4[boneCount];

        transform.position = Vector3.zero;

        GameObject model = new GameObject("Model");
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        root = new GameObject("Root").transform;
        root.SetParent(transform);
        root.localPosition = Vector3.zero;
        root.localRotation = Quaternion.identity;

        for(int i = 0; i < boneCount; i++)
        {
            GameObject boneInstance = new GameObject("Bone_" + i);
            boneInstance.transform.parent = root;
            boneTransforms[i] = boneInstance.transform;
        }

        skinnedMeshRenderer = model.AddComponent<SkinnedMeshRenderer>();
        skinnedMeshRenderer.sharedMesh = mesh = model.AddComponent<MeshFilter>().sharedMesh = new Mesh();
        skinnedMeshRenderer.rootBone = root.transform;
        skinnedMeshRenderer.updateWhenOffscreen = true;
        skinnedMeshRenderer.sharedMaterial = bodyMaterial;

        //if (!legGen)
        //{
        //    meshCollider = gameObject.AddComponent<MeshCollider>();
        //    meshCollider.convex = true;
        //}

        //if (legGen)
        //{ mesh.name = "Leg"; gameObject.tag = "Creature_Leg"; }
        //else
        //{ mesh.name = "Body"; gameObject.tag = "Creature_Body"; }
    }

    void Start()
    {
        CalculateVertices();
        CalculateTriangles();

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        CalculateBones();

        mesh.bindposes = bindPoses;
        skinnedMeshRenderer.bones = boneTransforms;
        mesh.boneWeights = boneWeights.ToArray();

        mesh.RecalculateNormals();

        //// enable shaping process
        //if (!legGen) GetComponent<BodyPrep>().enabled = true;
        //else GetComponent<LegPrep>().enabled = true;
    }

    void CalculateVertices()
    {
        // top dome
        vertices.Add(new Vector3(0, 0, 0));
        boneWeights.Add(new BoneWeight() { boneIndex0 = 0, weight0 = 1 });
        for (int ringIndex = 1; ringIndex < segments / 2; ringIndex++)
        {
            float percent = (float)ringIndex / (segments / 2);
            float ringRadius = radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);
            float ringDistance = radius * (-Mathf.Cos(90f * percent * Mathf.Deg2Rad) + 1f);

            for (int i = 0; i < segments; i++)
            {
                float angle = i * 360f / segments;

                float x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringDistance;

                vertices.Add(new Vector3(x, y, z));
                boneWeights.Add(new BoneWeight() { boneIndex0 = 0, weight0 = 1f });
            }
        }

        // middle
        for (int ringIndex = 0; ringIndex < rings * boneCount; ringIndex++)
        {
            float boneIndexFloat = (float)ringIndex / rings;
            int boneIndex = Mathf.FloorToInt(boneIndexFloat);

            float bonePercent = boneIndexFloat - boneIndex;

            int boneIndex0 = (boneIndex > 0) ? boneIndex - 1 : 0;
            int boneIndex2 = (boneIndex < boneCount - 1) ? boneIndex + 1 : boneCount - 1;
            int boneIndex1 = boneIndex;

            float weight0;
            float weight2;
            //if (legGen)
            //{
            //    weight0 = (boneIndex > 0) ? (1f - bonePercent) * 0.25f : 0f;
            //    weight2 = (boneIndex < boneCount - 1) ? bonePercent * 0.25f : 0f;
            //}
            //else
            //{
                weight0 = (boneIndex > 0) ? (1f - bonePercent) * 0.5f : 0f;
                weight2 = (boneIndex < boneCount - 1) ? bonePercent * 0.5f : 0f;
            //}
            float weight1 = 1f - (weight0 + weight2);

            for (int i = 0; i < segments; i++)
            {
                float angle = i * 360f / segments;

                float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringIndex * length / rings;

                vertices.Add(new Vector3(x, y, radius + z));
                boneWeights.Add(new BoneWeight()
                {
                    boneIndex0 = boneIndex0,
                    boneIndex1 = boneIndex1,
                    boneIndex2 = boneIndex2,
                    weight0 = weight0,
                    weight1 = weight1,
                    weight2 = weight2
                });
            }
        }

        // bottom dome
        for (int ringIndex = 0; ringIndex < segments / 2; ringIndex++)
        {
            float percent = (float)ringIndex / (segments / 2);
            float ringRadius = radius * Mathf.Cos(90f * percent * Mathf.Deg2Rad);
            float ringDistance = radius * Mathf.Sin(90f * percent * Mathf.Deg2Rad);

            for (int i = 0; i < segments; i++)
            {
                float angle = i * 360f / segments;

                float x = ringRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float y = ringRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float z = ringDistance;

                vertices.Add(new Vector3(x, y, radius + (length * boneCount) + z));
                boneWeights.Add(new BoneWeight() { boneIndex0 = boneCount - 1, weight0 = 1 });
            }
        }
        vertices.Add(new Vector3(0, 0, 2f * radius + (length * boneCount)));
        boneWeights.Add(new BoneWeight() { boneIndex0 = boneCount - 1, weight0 = 1 });
    }

    void CalculateTriangles()
    {
        // top cap
        for (int i = 0; i < segments; i++)
        {
            int seamOffset = i != segments - 1 ? 0 : segments;

            triangles.Add(0);
            triangles.Add(i + 2 - seamOffset);
            triangles.Add(i + 1);
        }

        // middle cap
        int ringCount = (rings * boneCount) + (2 * (segments / 2 - 1));
        for (int ringIndex = 0; ringIndex < ringCount; ringIndex++)
        {
            int ringOffset = 1 + ringIndex * segments;

            for (int i = 0; i < segments; i++)
            {
                int seamOffset = i != segments - 1 ? 0 : segments;

                triangles.Add(ringOffset + i);
                triangles.Add(ringOffset + i + 1 - seamOffset);
                triangles.Add(ringOffset + i + 1 - seamOffset + segments);

                triangles.Add(ringOffset + i + 1 - seamOffset + segments);
                triangles.Add(ringOffset + i + segments);
                triangles.Add(ringOffset + i);
            }
        }

        // bottom cap
        int topIndex = vertices.Count - 1;
        for (int i = 0; i < segments; i++)
        {
            int seamOffset = i != segments - 1 ? 0 : segments;

            triangles.Add(topIndex);
            triangles.Add(topIndex - i - 2 + seamOffset);
            triangles.Add(topIndex - i - 1);
        }
    }

    void CalculateBones()
    {
        Vector3[] deltaZeroArray = new Vector3[vertices.Count];
        for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
        {
            deltaZeroArray[vertIndex] = Vector3.zero;
        }

        for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
        {
            //if(legGen) boneTransforms[boneIndex].localPosition = Vector3.forward * (radius + length * (boneIndex == 0 ? 0.2f : boneIndex));
            //else
            boneTransforms[boneIndex].localPosition = Vector3.forward * (radius + length * (0.5f + boneIndex));
            boneTransforms[boneIndex].localRotation = Quaternion.identity;
            bindPoses[boneIndex] = boneTransforms[boneIndex].worldToLocalMatrix * transform.localToWorldMatrix;

            //if (boneIndex > 0)
            //{
            //    HingeJoint hingeJoint = boneTransforms[boneIndex].GetComponent<HingeJoint>();
            //    hingeJoint.anchor = new Vector3(0, 0, -length / 2f);
            //    hingeJoint.connectedBody = boneTransforms[boneIndex - 1].GetComponent<Rigidbody>();
            //}

            //Vector3[] deltaVertices = new Vector3[vertices.Count];
            Vector3[] deltaVerticesX = new Vector3[vertices.Count];
            Vector3[] deltaVerticesY = new Vector3[vertices.Count];
            for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
            {
                // Round
                //float distanceToBone = Mathf.Clamp(Vector3.Distance(vertices[vertIndex], boneTransforms[boneIndex].localPosition), 0, 2f * length);
                //Vector3 directionToBone = (vertices[vertIndex] - boneTransforms[boneIndex].localPosition).normalized;

                //deltaVertices[vertIndex] = directionToBone * (2f * length - distanceToBone);


                // Smooth - https://www.desmos.com/calculator/wmpvvtmor8
                float maxDistanceAlongBone = length * 2f;
                float maxHeightAboveBone = radius * 2f;

                float displacementAlongBone = vertices[vertIndex].z - boneTransforms[boneIndex].localPosition.z;

                float x = Mathf.Clamp(displacementAlongBone / maxDistanceAlongBone, -1, 1);
                float a = maxHeightAboveBone;
                float b = 1f / a;

                float heightAboveBone = (Mathf.Cos(x * Mathf.PI) / b + a) / 2f;

                float vertexDst = Mathf.Sqrt(Mathf.Pow(vertices[vertIndex].x, 2) + Mathf.Pow(vertices[vertIndex].y, 2));
                deltaVerticesX[vertIndex] = new Vector3(vertices[vertIndex].x / vertexDst, 0, 0) * heightAboveBone;
                deltaVerticesY[vertIndex] = new Vector3(0, vertices[vertIndex].y / vertexDst, 0) * heightAboveBone;
            }

            mesh.AddBlendShapeFrame("BoneX." + boneIndex, 0, deltaZeroArray, deltaZeroArray, deltaZeroArray);
            mesh.AddBlendShapeFrame("BoneX." + boneIndex, 100, deltaVerticesX, deltaZeroArray, deltaZeroArray);
            mesh.AddBlendShapeFrame("BoneY." + boneIndex, 0, deltaZeroArray, deltaZeroArray, deltaZeroArray);
            mesh.AddBlendShapeFrame("BoneY." + boneIndex, 100, deltaVerticesY, deltaZeroArray, deltaZeroArray);
        }

        for(int i = 1; i < boneCount; i++)
        {
            boneTransforms[i].parent = boneTransforms[i - 1];
        }

    }

    public void UpdateMeshCollider()
    {
        Mesh skinnedMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(skinnedMesh);
        meshCollider.sharedMesh = skinnedMesh;
    }

}
