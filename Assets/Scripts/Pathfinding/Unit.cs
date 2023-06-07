using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

    [SerializeField] float waypointDistanceThreshold = 5f;

    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    Vector3[] path;
    int targetIndex;
    public Transform target;
    public bool seekingTarget = false;

    LayerMask groundMask;

    DinosaurManager dinosaurManager;
    DinosaurSetup dinosaurSetup;

    Action<bool, Transform> approachCallback;

    private void Awake()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        groundMask |= 1 << groundLayer;
    }

    void Start()
    {
        dinosaurManager = transform.GetComponent<DinosaurManager>();
        dinosaurSetup = transform.GetComponent<DinosaurSetup>();

        StartCoroutine(UpdatePath());
    }

    public void SetTarget(Transform target, Action<bool, Transform> approachCallback)
    {
        this.target = target;
        this.approachCallback = approachCallback;
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
            bool success = newPath.Length == 0;

            approachCallback(success, target);
            StopCoroutine("FollowPath");
            DropTarget();
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
                    approachCallback(true, target);
                    DropTarget();
                    yield break;
                }
                else if (targetIndex == path.Length - 1 && target.tag == "Food")
                {
                    distanceThreshold = dinosaurSetup.Dinosaur.Reach * dinosaurManager.CurrentGrowth;
                }

				currentWaypoint = path[targetIndex];
			}

            dinosaurManager.Move(currentWaypoint);

            yield return null;
		}
	}

    void DropTarget()
    {
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
