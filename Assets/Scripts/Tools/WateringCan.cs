using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    [SerializeField] float useHeight = 10f;

    ParticleSystem water;
    CapsuleCollider canCollider;
    bool isUsed = false;

    private void Awake()
    {
        water = GetComponent<ParticleSystem>();
        canCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (isUsed)
            UseWateringCan();
    }

    public void StartWatering()
    {
        isUsed = true;
        canCollider.enabled = true;
    }

    public void StopWatering()
    {
        isUsed = false;
        canCollider.enabled = false;
    }

    public void UpdateCanPos(Vector3 mousePos)
    {
        transform.position = mousePos + useHeight * Vector3.up;
    }

    public void UseWateringCan()
    {
        water.Play();
    }
}
