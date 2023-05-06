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

    Transform hint;

    private void Awake()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        terrainLayer |= (1 << groundLayer);
    }

    private void Start()
    {
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
        string otherFootName = "2BoneIK_Leg_" + (transform.parent.name.EndsWith("L") ? "R" : "L");
        return transform.parent.parent.Find(otherFootName).Find("Target").GetComponent<IKFootSolver>();
    }

    void SetHintPosition()
    {
        TwoBoneIKConstraint legIKConstraint = transform.parent.GetComponent<TwoBoneIKConstraint>();
        Transform hint = legIKConstraint.data.hint;
        hint.position = legRoot.position + new Vector3(0, 0, 1000f);
    }

    // Update is called once per frame

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

            // put into separate script
            float currBobHeight = Mathf.Sin(lerp * Mathf.PI) * -bodyBobAmount;
            bodyRoot.localPosition = new Vector3(bodyRoot.localPosition.x, currBobHeight, bodyRoot.localPosition.z);
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
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
