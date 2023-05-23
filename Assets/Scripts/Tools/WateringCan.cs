using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    [SerializeField] float useHeight = 10f;

    ParticleSystem water;

    private void Awake()
    {
        water = GetComponent<ParticleSystem>();
    }

    public void UseWateringCan(Vector3 usePos)
    {
        transform.position = usePos + useHeight * Vector3.up;
        water.Play();
    }
}
