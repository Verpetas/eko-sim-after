using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FruitGrabber : MonoBehaviour
{
    public ChainIKConstraint neckIK;
    public float neckSpeed = 0.05f;

    private void Update()
    {
        UpdateDinosaurNeck();    
    }

    void UpdateDinosaurNeck()
    {
        if (Input.GetKey("[1]"))
        {
            neckIK.weight += neckSpeed;
        }

        if (Input.GetKey("[2]"))
        {
            neckIK.weight -= neckSpeed;
        }
    }

}
