using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dinosaur", menuName = "Dinosaur")]
public class Dinosaur : ScriptableObject
{

    public string speciesName;
    public bool bipedal;
    public float[] spineBends;
    public float[] spineStretches;
    public float[] legStretches;

}
