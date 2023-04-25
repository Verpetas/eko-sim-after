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
    WidthHeredity[] widthHeredity; // false means second dino, true - first

    private void Awake()
    {
        meshGen = GetComponent<MeshGen>();
        genes = new Gene[boneCount];
        widthHeredity = new WidthHeredity[boneCount];
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
            widthHeredity[i] = new WidthHeredity(Random.value < 0.5f, Random.value < 0.5f);
        }
    }

    void DetermineGenes()
    {
        for (int i = 0; i < boneCount; i++)
        {
            float spineBendVal = (Random.value < 0.5f) ? dinosaur1st.spineBends[i] : dinosaur2nd.spineBends[i];
            float widthValXPassed, widthValYPassed, widthValXActual, widthValYActual;

            widthValXPassed = (widthHeredity[i].x) ? dinosaur1st.spineStretchesX[i] : dinosaur2nd.spineStretchesX[i];
            widthValYPassed = (widthHeredity[i].y) ? dinosaur1st.spineStretchesY[i] : dinosaur2nd.spineStretchesY[i];

            if (i > 0 && widthHeredity[i].x != widthHeredity[i - 1].x)
                widthValXActual = (dinosaur1st.spineStretchesX[i] + dinosaur2nd.spineStretchesX[i]) / 2;
            else
                widthValXActual = widthValXPassed;

            if (i > 0 && widthHeredity[i].y != widthHeredity[i - 1].y)
                widthValYActual = (dinosaur1st.spineStretchesY[i] + dinosaur2nd.spineStretchesY[i]) / 2;
            else
                widthValYActual = widthValYPassed;

            genes[i] = new Gene(spineBendVal, widthValXPassed, widthValYPassed, widthValXActual, widthValYActual);
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
        meshGen.boneTransforms[boneIndex].localRotation = Quaternion.Euler(genes[boneIndex].spineBendVal + Random.Range(-bendRandomness, bendRandomness), 0, 0);
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

public struct WidthHeredity
{
    public bool x { get; }
    public bool y { get; }

    public WidthHeredity(bool x, bool y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Gene
{
    public float spineBendVal { get; }

    public float thicknessValXPassed { get; }
    public float thicknessValYPassed { get; }
    public float thicknessValXActual { get; }
    public float thicknessValYActual { get; }

    public Gene(float spineBendVal, float thicknessValXPassed, float thicknessValYPassed, float thicknessValXActual, float thicknessValYActual)
    {
        this.spineBendVal = spineBendVal;

        this.thicknessValXPassed = thicknessValXPassed;
        this.thicknessValYPassed = thicknessValYPassed;

        this.thicknessValXActual = thicknessValXActual;
        this.thicknessValYActual = thicknessValYActual;
    }
}
