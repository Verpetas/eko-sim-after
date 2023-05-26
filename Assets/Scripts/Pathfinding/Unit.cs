using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    const float moveForce = 10000f;

	public float waypointDistanceThreshold = 5f; 
	public Transform target;
	public float speed = 20f;
	public float turnSpeed = 0.5f;
	public LayerMask groundMask;
    public bool seekingTarget = false;

    Vector3[] path;
	int targetIndex;
	Rigidbody seekerRB;
    DinosaurManager dinosaurManager;
    DinosaurSetup dinosaurSetup;

    Action<bool, Transform> foodApproachCallback;

    void Start()
    {
        seekerRB = transform.GetComponent<Rigidbody>();
        dinosaurManager = transform.GetComponent<DinosaurManager>();
        dinosaurSetup = transform.GetComponent<DinosaurSetup>();

        StartCoroutine(UpdatePath());
    }

    public void SetFoodTarget(Transform target, Action<bool, Transform> approachCallback)
    {
        this.target = target;
        this.foodApproachCallback = approachCallback;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
		if (pathSuccessful)
        {
			path = newPath;
			targetIndex = 0;
            seekingTarget = true;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
        else
        {
            StopCoroutine("FollowPath");
            DropTarget(false);
        }
	}

	IEnumerator UpdatePath()
	{
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = Vector3.zero;

		while (true)
		{
            yield return new WaitForSeconds(minPathUpdateTime);

            if (target != null)
            {
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }
	}

	IEnumerator FollowPath()
	{
        float distanceThreshold = waypointDistanceThreshold;
		Vector3 currentWaypoint = path[0];
		while (true)
		{
            Vector3 groundPoint = new Vector3();
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                groundPoint = hit.point;
            }

            if (Vector3.Distance(groundPoint, currentWaypoint) < distanceThreshold)
            {
                targetIndex ++;
                if (targetIndex >= path.Length)
                {
                    DropTarget(true);
                    yield break;
                }
                else if (targetIndex == path.Length - 1)
                {
                    distanceThreshold = dinosaurSetup.Dinosaur.Reach;
                }

				currentWaypoint = path[targetIndex];
			}

            if (dinosaurManager.touchingGround)
            {
                TurnTowardsWaypoint(currentWaypoint);

                if (seekerRB.velocity.sqrMagnitude < speed)
                    seekerRB.AddForce(DirectForce(transform.forward) * moveForce * Time.deltaTime);
            }

            yield return null;

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

    Vector3 DirectForce(Vector3 waypoint)
    {
        Vector3 bodyUp = transform.up;
        RaycastHit hit;
        Physics.Raycast(transform.position, -bodyUp, out hit, 500f, groundMask);

        Vector3 bodyDirection = Vector3.ProjectOnPlane(waypoint, bodyUp);
        Vector3 forceDirection = (Quaternion.FromToRotation(bodyUp, hit.normal) * bodyDirection).normalized;

        return forceDirection;
    }

    void DropTarget(bool success)
    {
        if (target.tag == "Food")
            foodApproachCallback(success, target);

        target = null;
        path = null;
        seekingTarget = false;
    }

    public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
