using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{

    public float gravity = -10;

    Rigidbody rb;
    CapsuleCollider collider;
    Vector3 colliderExtents;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //colliderExtents = collider.bounds.extents;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //float distToGround = colliderExtents.y;
        //rb.useGravity = !Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, distToGround * 1.01f);
    }
}
