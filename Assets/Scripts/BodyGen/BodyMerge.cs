using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BodyMerge : MonoBehaviour
{

    public Dinosaur dinosaur1st;
    public Dinosaur dinosaur2nd;
    public float bendRandomness = 15f;
    public float stretchRandomness = 0.4f;

    MeshGen meshGen;
    static int boneCount = 23;
    Gene[] genes;
    Heredity[] heredity; // true means first dino, false - second

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        genes = new Gene[boneCount];
        heredity = new Heredity[boneCount];
    }

    public void FormOffspring()
    {
        DetermineHeredity();
        DetermineGenes();
        ShapeBody();

        //AdjustRotation();
    }

    void DetermineHeredity()
    {
        for (int i = 0; i < boneCount; i++)
        {
            heredity[i] = new Heredity(Random.value < 0.5f, Random.value < 0.5f, Random.value < 0.5f);
        }
    }

    void DetermineGenes()
    {
        for (int i = 0; i < boneCount; i++)
        {
            float localBendVal = heredity[i].bend ? dinosaur1st.spineBends[i] : dinosaur2nd.spineBends[i];

            if (i == 0)
            {
                genes[i] = new Gene(localBendVal, 1f, 1f);
                continue;
            }

            float relativeWidthValX;
            if (heredity[i].widthX)
                relativeWidthValX = dinosaur1st.spineStretchesX[i] - dinosaur1st.spineStretchesX[i - 1];
            else
                relativeWidthValX = dinosaur2nd.spineStretchesX[i] - dinosaur2nd.spineStretchesX[i - 1];

            float relativeWidthValY;
            if (heredity[i].widthY)
                relativeWidthValY = dinosaur1st.spineStretchesY[i] - dinosaur1st.spineStretchesY[i - 1];
            else
                relativeWidthValY = dinosaur2nd.spineStretchesY[i] - dinosaur2nd.spineStretchesY[i - 1];


            genes[i] = new Gene(localBendVal, relativeWidthValX, relativeWidthValY);
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
        meshGen.boneTransforms[boneIndex].localRotation = Quaternion.Euler(genes[boneIndex].localBendVal + Random.Range(-bendRandomness, bendRandomness), 0, 0);
    }

    void StretchBody(int boneIndex)
    {
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2, genes[boneIndex].thicknessValXActual * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
        meshGen.skinnedMeshRenderer.SetBlendShapeWeight(boneIndex * 2 + 1, genes[boneIndex].thicknessValYActual * Random.Range(1 - stretchRandomness, 1 + stretchRandomness));
    }

    void AdjustRotation()
    {
        Quaternion rotStart = Quaternion.Euler(0, 0, 0);
        Quaternion rotEnd = meshGen.boneTransforms[boneCount - 1].rotation;
        meshGen.boneTransforms[0].rotation = Quaternion.Inverse(Quaternion.Slerp(rotStart, rotEnd, 0.5f));
    }

}

public struct Heredity
{
    public bool bend { get; }
    public bool widthX { get; }
    public bool widthY { get; }

    public Heredity(bool bend, bool widthX, bool widthY)
    {
        this.bend = bend;
        this.widthX = widthX;
        this.widthY = widthY;
    }
}

public class Gene
{
    public float localBendVal { get; }

    public float relativeWidthValX { get; }
    public float relativeWidthValY { get; }

    public Gene(float localBendVal, float relativeWidthValX, float relativeWidthValY)
    {
        this.localBendVal = localBendVal;
        this.relativeWidthValX = relativeWidthValX;
        this.relativeWidthValY = relativeWidthValY;
    }
}
