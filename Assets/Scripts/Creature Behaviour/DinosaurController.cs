using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinosaurController : MonoBehaviour
{
    [SerializeField] float speed = 1500f;
    [SerializeField] float gravity = -10f;
    [SerializeField] bool touchingGround = false;

    float groundedDrag = 1;
    Rigidbody rb;

    public void InitController()
    {
        AddRB();
    }

    void Update()
    {
        UpdateDinosaurPos();
        UpdateDinosaurRot();
        ApplyGravity();
    }

    void UpdateDinosaurPos()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(transform.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(-transform.forward * speed * Time.deltaTime);
        }
    }

    void UpdateDinosaurRot()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(-Vector3.up * 100 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        rb.AddForce(transform.up * gravity);
    }

    public void AddCollider(float radius, float height)
    {
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.radius = radius;
        collider.height = height;
    }

    void AddRB()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
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

}
