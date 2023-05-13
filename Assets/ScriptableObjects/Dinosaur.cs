using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dinosaur", menuName = "Dinosaur")]
public class Dinosaur : ScriptableObject
{
    public bool bipedal;
    public List<int> legBoneIndices;
    public List<float> legPairSizes;
    public float bodySize;

    public int tailLength;
    public int neckLength;

    public float[] spineBends;

    public Vector2[] spineWidths;
    public Vector2[] legWidths;

    public DinosaurWalkingProperties walkingProperties;
}

[Serializable]
public class DinosaurWalkingProperties
{
    public float stepSpeed;
    public float stepDistance;
    public float stepLength;
    public float stepHeight;
    public Vector3 footOffset;
    public float bodyBobAmount;
}