using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DinosaurManager : MonoBehaviour
{
    [SerializeField] float neckSpeed = 1f; 

    public bool touchingGround = false;
    float gravityFalling = -5f;
    float dragGrounded = 1;

    float gravity;
    Rigidbody rb;

    Unit unitInstance;
    VegetationManager vegetationManager;
    PopulationManager populationManager;

    bool hungry = true;
    bool searching = false;

    ChainIKConstraint neckIK;

    private void Awake()
    {
        unitInstance = transform.GetComponent<Unit>();
        populationManager = GameObject.FindWithTag("PopulationManager").GetComponent<PopulationManager>();
        vegetationManager = GameObject.FindWithTag("VegetationManager").GetComponent<VegetationManager>();

        gravity = gravityFalling;
    }

    private void Start()
    {
        StartCoroutine(TryFindFood());
        StartCoroutine(CheckReadyToPair());
    }

    void Update()
    {
        ApplyGravity();
    }

    IEnumerator TryFindFood()
    {
        while (true)
        {
            int foodInstanceCount = vegetationManager.AvailableFood.Count;

            if (foodInstanceCount > 0 && hungry && !searching)
            {
                int randomFoodIndex = Random.Range(0, foodInstanceCount);
                Transform foundFood = vegetationManager.AvailableFood[randomFoodIndex];

                RouteToFood(foundFood);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void RouteToFood(Transform food)
    {
        vegetationManager.RemoveFoodFromAvailable(food);

        unitInstance.SetTarget(food, OnFoodApproach);
        searching = true;
    }

    void OnFoodApproach(bool success, Transform food)
    {
        if (success)
        {
            rb.velocity = Vector3.zero;
            StartCoroutine(PickFood(food));
        }
        else
            searching = false;
    }

    IEnumerator PickFood(Transform food)
    {
        neckIK.weight = 0;
        neckIK.data.target.position = food.position;

        Vector3 headStartPos = neckIK.data.tip.position;
        Vector3 headEndPos = food.position;

        float distance = Vector3.Distance(headStartPos, headEndPos);

        // reach towards food
        float startTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time - startTime;
            neckIK.data.target.position = food.position;
            neckIK.weight = (elapsedTime * neckSpeed) / distance;

            if (neckIK.weight >= 1)
                break;
            else
                yield return null;
        }
        neckIK.weight = 1;

        // take food
        startTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time - startTime;
            neckIK.weight = 1 - (elapsedTime * neckSpeed) / distance;

            Transform head = neckIK.data.tip;
            food.position = head.TransformPoint(Vector3.forward * 10f);

            if (neckIK.weight <= 0)
                break;
            else
                yield return null;
        }
        neckIK.weight = 0;
        neckIK.data.target.position = neckIK.data.tip.position;

        EatFood(food);
    }

    IEnumerator CheckReadyToPair()
    {
        while (true)
        {
            if (!hungry && !searching)
            {
                populationManager.AddDinosaurToAvailable(transform);
                searching = true;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void EatFood(Transform food)
    {
        Destroy(food.gameObject);

        hungry = false;
        searching = false;
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

    public ChainIKConstraint NeckIK
    {
        set { neckIK = value; }
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
