using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DinosaurManager : MonoBehaviour
{
    [SerializeField] float gravity = -10f;
    public bool touchingGround = false;

    Unit unitInstance;

    float groundedDrag = 1;
    Rigidbody rb;

    private void Awake()
    {
        unitInstance = transform.GetComponent<Unit>();
    }

    public void PrepareDinosaur()
    {
        AssignSpawnPosition();
        AddRB();
        EnablePathfinding();
    }

    void Update()
    {
        ApplyGravity();
    }

    public void AddCollider(float radius, float height)
    {
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.radius = radius;
        collider.height = height;
    }

    void AssignSpawnPosition()
    {
        DinosaurSetup dinosaurSetup = transform.GetComponent<DinosaurSetup>();
        transform.position = dinosaurSetup.SpawnPos + Vector3.up * 10f;
        transform.rotation = dinosaurSetup.SpawnRot;
    }

    void AddRB()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    void EnablePathfinding()
    {
        unitInstance.enabled = true;
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
            rb.drag = groundedDrag;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            touchingGround = false;
            rb.drag = 0;
        }
    }

    //void UpdateDinosaurPos()
    //{
    //    if (Input.GetKey(KeyCode.UpArrow))
    //    {
    //        rb.AddForce(transform.forward * speed * Time.deltaTime);
    //    }

    //    if (Input.GetKey(KeyCode.DownArrow))
    //    {
    //        rb.AddForce(-transform.forward * speed * Time.deltaTime);
    //    }
    //}

    //void UpdateDinosaurRot()
    //{
    //    if (Input.GetKey(KeyCode.LeftArrow))
    //    {
    //        transform.Rotate(-Vector3.up * 100 * Time.deltaTime);
    //    }
    //    if (Input.GetKey(KeyCode.RightArrow))
    //    {
    //        transform.Rotate(Vector3.up * 100 * Time.deltaTime);
    //    }
    //}

}
