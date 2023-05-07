using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCheck : MonoBehaviour
{
    GameObject sphere;

    private void Awake()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 50f;
    }

    private void Update()
    {
        sphere.transform.position = transform.position;
    }
}
