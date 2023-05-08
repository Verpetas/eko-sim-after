using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp;

    // added
    Vector3 gizmoPoint;

    [SerializeField] float bodyBobAmount;
    public Transform bodyRoot;
    public Transform legRoot;

    string legIKName;
    bool backLeg;

    private void Awake()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        terrainLayer |= (1 << groundLayer);
    }

    private void Start()
    {
        legIKName = transform.parent.name;
        backLeg = legIKName[legIKName.Length - 1] == '0';
        otherFoot = FindOtherFoot();
        SetHintPosition();

        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }

    public void AssignWalkProperties(float speed, float stepDistance, float stepLength, float stepHeight, Vector3 footOffset, float bodyBobAmount)
    {
        this.speed = speed;
        this.stepDistance = stepDistance;
        this.stepLength = stepLength;
        this.stepHeight = stepHeight;
        this.footOffset = footOffset;
        this.bodyBobAmount = bodyBobAmount;
    }

    IKFootSolver FindOtherFoot()
    {
        char legPairNo = legIKName[legIKName.Length - 1];
        char otherLegSide = legIKName[legIKName.Length - 3] == 'L' ? 'R' : 'L';
        string otherLegName = "2BoneIK_Leg_" + otherLegSide + "_" + legPairNo;

        return transform.parent.parent.Find(otherLegName).Find("Target").GetComponent<IKFootSolver>();
    }

    void SetHintPosition()
    {
        Transform hint = transform.parent.Find("Hint");
        hint.position = legRoot.position + new Vector3(0, 0, 1000f);

        if (legIKName[legIKName.Length - 1] == '1') // if leg belongs to front leg pair
            hint.position *= -1f;
    }

    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        //Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        Ray ray = new Ray(legRoot.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, Mathf.Infinity, terrainLayer.value))
        {
            gizmoPoint = info.point;

            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                int direction = bodyRoot.InverseTransformPoint(info.point).z > bodyRoot.InverseTransformPoint(newPosition).z ? 1 : -1;

                Vector3 rayStartFinal = legRoot.position + (bodyRoot.forward * stepLength * direction);
                ray = new Ray(rayStartFinal, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit infoFinal, Mathf.Infinity, terrainLayer.value))
                {
                    newPosition = infoFinal.point + footOffset;
                    newNormal = infoFinal.normal;

                    lerp = 0;
                }
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;

            if (backLeg) PerformBodyBob(lerp);
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    void PerformBodyBob(float bobStage)
    {
        float currBobHeight = Mathf.Sin(bobStage * Mathf.PI) * -bodyBobAmount;
        bodyRoot.localPosition = new Vector3(bodyRoot.localPosition.x, currBobHeight, bodyRoot.localPosition.z);
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(gizmoPoint, 5f);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }



}
