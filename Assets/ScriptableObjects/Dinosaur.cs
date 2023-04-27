using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dinosaur", menuName = "Dinosaur")]
public class Dinosaur : ScriptableObject
{

    public bool bipedal;

    public float[] spineBends;
    public float[] spineBendsGlobal;

    public float[] spineStretchesX;
    public float[] spineStretchesY;

}
