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
    [SerializeField] float growSpeed = 1f;
    [SerializeField] float subsequentGrowAmountRatio = 1.05f;
    public float currentGrowthAmount = 1f;

    [SerializeField] float speed = 20f;
    [SerializeField] float turnSpeed = 0.5f;
    const float moveForce = 10000f;

    public bool touchingGround = false;
    float gravityFalling = -5f;
    float dragGrounded = 1;

    LayerMask groundMask;

    float gravity;
    Rigidbody rb;

    Unit unitInstance;
    VegetationManager vegetationManager;
    PopulationManager populationManager;

    bool hungry = true;
    bool searching = false;

    ChainIKConstraint neckIK;

    BodyPrep body;
    LegPrep[] legs;

    private void Awake()
    {
        unitInstance = transform.GetComponent<Unit>();
        populationManager = GameObject.FindWithTag("PopulationManager").GetComponent<PopulationManager>();
        vegetationManager = GameObject.FindWithTag("VegetationManager").GetComponent<VegetationManager>();

        gravity = gravityFalling;

        int groundLayer = LayerMask.NameToLayer("Ground");
        groundMask |= 1 << groundLayer;
    }

    private void Start()
    {
        body = gameObject.GetComponentsInChildren<BodyPrep>()[0];
        legs = gameObject.GetComponentsInChildren<LegPrep>();

        StartCoroutine(Grow());
        StartCoroutine(TryFindFood());
        StartCoroutine(CheckReadyToPair());

        UpdateDinosaurScale(currentGrowthAmount);
        MakeBodyVisible();
    }

    void Update()
    {
        ApplyGravity();
    }

    IEnumerator Grow()
    {
        while (true)
        {
            yield return null; //new WaitForSeconds(1f / growSpeed);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                currentGrowthAmount *= subsequentGrowAmountRatio;
                UpdateDinosaurScale(currentGrowthAmount);
            }
        }
    }

    void UpdateDinosaurScale(float scale)
    {
        transform.localScale = Vector3.one * scale;

        body.UpdateTail(scale);

        foreach (LegPrep leg in legs)
        {
            leg.UpdateLegs(scale);
        }
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

    public void Move(Vector3 waypoint)
    {
        if (touchingGround)
        {
            TurnTowardsWaypoint(waypoint);

            if (rb.velocity.sqrMagnitude < speed * currentGrowthAmount)
                rb.AddForce(DirectMoveForce(transform.forward) * moveForce * Time.deltaTime);
        }
    }

    void TurnTowardsWaypoint(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        Vector3 targetDir = Vector3.ProjectOnPlane(direction, transform.up);
        float angle = Vector3.SignedAngle(targetDir, transform.forward, transform.up);

        if (angle > 0.5f)
        {
            transform.rotation = Quaternion.AngleAxis(-turnSpeed, transform.up) * transform.rotation;
        }
        else if (angle < -0.5f)
        {
            transform.rotation = Quaternion.AngleAxis(turnSpeed, transform.up) * transform.rotation;
        }
    }

    Vector3 DirectMoveForce(Vector3 waypoint)
    {
        Vector3 bodyUp = transform.up;
        RaycastHit hit;
        Physics.Raycast(transform.position, -bodyUp, out hit, 500f, groundMask);

        Vector3 bodyDirection = Vector3.ProjectOnPlane(waypoint, bodyUp);
        Vector3 forceDirection = (Quaternion.FromToRotation(bodyUp, hit.normal) * bodyDirection).normalized;

        return forceDirection;
    }

    public void FinishPairing()
    {
        Debug.Log("Finished pairing");
        //hungry = true;
        //searching = false;
    }

    void MakeBodyVisible()
    {
        body.MeshRenderer.enabled = true;
        foreach (LegPrep leg in legs)
        {
            leg.MeshRenderer.enabled = true;
        }
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
