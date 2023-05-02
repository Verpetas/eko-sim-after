using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPrep : MonoBehaviour
{

    public Dinosaur dinosaur;

    MeshGen meshGen;
    int boneCount;

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

    }

    void StretchLeg(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, dinosaur.legWidths[boneIndex].x);
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, dinosaur.legWidths[boneIndex].y);
    }

}
