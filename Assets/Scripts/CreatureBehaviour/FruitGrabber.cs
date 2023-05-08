using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FruitGrabber : MonoBehaviour
{
    [SerializeField] ChainIKConstraint neckIK;
    [SerializeField] Transform apple;
    [SerializeField] Transform apple2;
    [SerializeField] float neckSpeed = 0.05f;

    RigBuilder rigBuilder;

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

        if (Input.GetKey("[4]"))
        {
            Debug.Log("Adding");

            rigBuilder = transform.Find("Wrapper").Find("Root").GetComponent<RigBuilder>();

            neckIK.data.target = apple;
            rigBuilder.Build();
        }

        if (Input.GetKey("[5]"))
        {
            Debug.Log("Adding");

            rigBuilder = transform.Find("Wrapper").Find("Root").GetComponent<RigBuilder>();

            neckIK.data.target = apple2;
            rigBuilder.Build();
        }
    }

}
