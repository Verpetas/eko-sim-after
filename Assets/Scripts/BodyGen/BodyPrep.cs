using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPrep : MonoBehaviour
{

    public Dinosaur dinosaur;
    public float bendRandomness = 15f;
    public float stretchRandomness = 0.4f;

    MeshGen meshGen;
    int boneCount;
    float[] globalBends;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        boneCount = dinosaur.spineBends.Length;

        globalBends = new float[boneCount];
        CalculateGlobalRots();
    }

    void CalculateGlobalRots()
    {
        globalBends[0] = 360f + dinosaur.spineBends[0];

        for (int i = 1; i < boneCount; i++)
        {
            globalBends[i] = globalBends[i - 1] + dinosaur.spineBends[i];
        }
    }

    public void ShapeBody()
    {
        for (int i = 0; i < boneCount; i++)
        {
            BendBody(i);
            StretchBody(i);
        }

        //AdjustRotation();
    }

    void BendBody(int boneIndex)
    {
        //meshGen.boneTransforms[boneIndex].localRotation = Quaternion.Euler(dinosaur.spineBends[boneIndex] + Random.Range(-bendRandomness, bendRandomness), 0, 0);
        meshGen.boneTransforms[boneIndex].rotation = Quaternion.Euler(globalBends[boneIndex] + Random.Range(-bendRandomness, bendRandomness), 0, 0);
    }

    void StretchBody(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, dinosaur.spineStretchesX[boneIndex] * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, dinosaur.spineStretchesY[boneIndex] * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
    }

    void AdjustRotation()
    {
        Quaternion rotStart = Quaternion.Euler(0, 0, 0);
        Quaternion rotEnd = meshGen.boneTransforms[boneCount - 1].rotation;
        meshGen.boneTransforms[0].rotation = Quaternion.Inverse(Quaternion.Slerp(rotStart, rotEnd, 0.5f));
    }

}
