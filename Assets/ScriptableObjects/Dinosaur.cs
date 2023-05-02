using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dinosaur", menuName = "Dinosaur")]
public class Dinosaur : ScriptableObject
{
    public bool bipedal;
    public List<int> legBoneIndices;

    public int tailLength;

    public float[] spineBends;

    public float[] spineWidthsX;
    public float[] spineWidthsY;

    public Vector2[] legWidths;
}
