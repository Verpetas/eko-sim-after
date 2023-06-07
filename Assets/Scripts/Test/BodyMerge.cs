using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BodyMerge : MonoBehaviour
{

    public Dinosaur dinosaur1st;
    public Dinosaur dinosaur2nd;
    public int tailLength = 11;
    public float bendRandomness = 15f;
    public float stretchRandomness = 0.4f;

    MeshGen meshGen;
    static int boneCount = 23;
    Gene[] genes;

    float[] globalBends1st;
    float[] globalBends2nd;

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        genes = new Gene[boneCount];

        globalBends1st = new float[boneCount];
        globalBends2nd = new float[boneCount];
    }

    public void FormOffspring()
    {
        CalculateGlobalRots();

        DetermineGenes();
        ShapeBody();

        //AdjustRotation();

        //RestructureBones();
    }

    void CalculateGlobalRots()
    {
        globalBends1st[0] = 360f + dinosaur1st.spineBends[0];
        globalBends2nd[0] = 360f + dinosaur2nd.spineBends[0];

        for (int i = 1; i < boneCount; i++)
        {
            globalBends1st[i] = globalBends1st[i - 1] + dinosaur1st.spineBends[i];
            globalBends2nd[i] = globalBends2nd[i - 1] + dinosaur2nd.spineBends[i];
        }
    }

    void DetermineGenes()
    {
        float heredityRatio;

        for (int i = 0; i < boneCount; i++)
        {
            heredityRatio = Random.Range(0.25f, 0.75f);
            float bendVal = globalBends1st[i] * heredityRatio + globalBends2nd[i] * (1 - heredityRatio);

            heredityRatio = Random.Range(0.25f, 0.75f);
            float widthValX = dinosaur1st.spineWidths[i].x * heredityRatio + dinosaur2nd.spineWidths[i].x * (1 - heredityRatio);

            heredityRatio = Random.Range(0.25f, 0.75f);
            float widthValY = dinosaur1st.spineWidths[i].y * heredityRatio + dinosaur2nd.spineWidths[i].y * (1 - heredityRatio);

            genes[i] = new Gene(bendVal, widthValX, widthValY);
        }
    }

    void ShapeBody()
    {
        for (int i = 0; i < boneCount; i++)
        {
            BendBody(i);
            StretchBody(i);
        }
    }

    void BendBody(int boneIndex)
    {
        meshGen.boneTransforms[boneIndex].rotation = Quaternion.Euler(genes[boneIndex].bendVal + Random.Range(-bendRandomness, bendRandomness), 0, 0);
    }

    void StretchBody(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, genes[boneIndex].widthValX * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, genes[boneIndex].widthValY * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
    }

    void RestructureBones()
    {
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

    void AdjustRotation()
    {
        Quaternion rotStart = Quaternion.Euler(0, 0, 0);
        Quaternion rotEnd = meshGen.boneTransforms[boneCount - 1].rotation;
        meshGen.boneTransforms[0].rotation = Quaternion.Inverse(Quaternion.Slerp(rotStart, rotEnd, 0.5f));
    }

}

public class Gene
{
    public float bendVal { get; }

    public float widthValX { get; }
    public float widthValY { get; }

    public Gene(float bendVal, float widthValX, float widthValY)
    {
        this.bendVal = bendVal;
        this.widthValX = widthValX;
        this.widthValY = widthValY;
    }
}
