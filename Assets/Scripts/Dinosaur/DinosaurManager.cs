using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DinosaurManager : MonoBehaviour
{
    [SerializeField] float gravityFalling = -10f;
    [SerializeField] float dragGrounded = 1;
    public bool touchingGround = false;

    float gravity;
    Rigidbody rb;
    Unit unitInstance;

    private void Awake()
    {
        unitInstance = transform.GetComponent<Unit>();
        gravity = gravityFalling;
    }

    void Update()
    {
        ApplyGravity();
    }

    public void EnablePathfinding()
    {
        unitInstance.enabled = true;
    }

    public void AddRB()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    void ApplyGravity()
    {
        rb.AddForce(transform.up * gravity);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            touchingGround = true;
            rb.drag = dragGrounded;
            gravity = 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            touchingGround = false;
            rb.drag = 0;
            gravity = gravityFalling;
        }
    }

}
