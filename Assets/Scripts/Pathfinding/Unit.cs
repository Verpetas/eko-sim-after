using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

	public float waypointDistanceThreshold = 1.5f; 

	public Transform target;
	public float moveForce = 1750f;
	public float turnSpeed = 0.5f;
	public LayerMask groundMask;
    public bool touchingGround = false;
    public float groundedDrag = 1;

    Vector3[] path;
	int targetIndex;
	Rigidbody seekerRB;

    private void Awake()
    {
        seekerRB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator UpdatePath()
	{
		if(Time.timeSinceLevelLoad < 0.3f)
		{
			yield return new WaitForSeconds(0.3f);
		}
        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true)
		{
            yield return new WaitForSeconds(minPathUpdateTime);
            //yield return null;
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
			{
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
				targetPosOld = target.position;
            }
        }
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];
		while (true)
		{
            Vector3 groundPoint = new Vector3();
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 500f, groundMask))
            {
                groundPoint = hit.point;
            }

            if (Vector3.Distance(groundPoint, currentWaypoint) < waypointDistanceThreshold)
            {
                targetIndex ++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

            //transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
            if (touchingGround)
            {
                TurnTowardsWaypoint(currentWaypoint);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			touchingGround = true;
            seekerRB.drag = groundedDrag;
		}
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            touchingGround = false;
            seekerRB.drag = 0;
        }
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
