using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dinosaur", menuName = "Dinosaur")]
public class Dinosaur : ScriptableObject
{
    public bool bipedal;
    public List<int> legBoneIndices;
    public List<float> legPairSizeRatios;
    public Vector2 colliderSize;
    public float bodySize;

    public int tailLength;
    public int neckLength;

    public float[] spineBends;

    public Vector2[] spineWidths;
    public Vector2[] legWidths;

    public Color skinColor;

    public DinosaurWalkingProperties walkingProperties;

    public void Init(bool bipedal, List<int> legBoneIndices, List<float> legPairSizeRatios, Vector2 colliderSize, float bodySize, int tailLength, int neckLength, float[] spineBends, Vector2[] spineWidths, Vector2[] legWidths, Color skinColor, DinosaurWalkingProperties walkingProperties)
    {
        this.bipedal = bipedal;
        this.legBoneIndices = legBoneIndices;
        this.legPairSizeRatios = legPairSizeRatios;
        this.colliderSize = colliderSize;
        this.bodySize = bodySize;
        this.tailLength = tailLength;
        this.neckLength = neckLength;
        this.spineBends = spineBends;
        this.spineWidths = spineWidths;
        this.legWidths = legWidths;
        this.skinColor = skinColor;
        this.walkingProperties = walkingProperties;
    }

    public float Reach
    {
        get { return neckLength * bodySize * 10f; }
    }
}

[Serializable]
public class DinosaurWalkingProperties
{
    public float stepSpeed;
    public float stepDistance;
    public float stepLength;
    public float stepHeight;
    public float bodyBobAmount;

    public DinosaurWalkingProperties(float stepSpeed, float stepDistance, float stepLength, float stepHeight, float bodyBobAmount)
    {
        this.stepSpeed = stepSpeed;
        this.stepDistance = stepDistance;
        this.stepLength = stepLength;
        this.stepHeight = stepHeight;
        this.bodyBobAmount = bodyBobAmount;
    }

}