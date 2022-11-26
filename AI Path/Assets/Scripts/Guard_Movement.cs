using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard_Movement : MonoBehaviour {

	// Initialize Action event delegate 
	public static event System.Action OnGuardHasSpottedPlayer;

	// Initialize Variable 
	public float speed = 5;
	public float waitTime = .3f;
	public float turnSpeed = 90;
	public float timeToSpotPlayer = .5f;
	public Light spotlight;
	public float viewDistance;
	public LayerMask viewMask;
	float viewAngle;
	float playerVisibleTimer;
	public Transform pathHolder;
	Transform player;
	Color originalSpotlightColour;

	// Start Function
	void Start() {
		// FindGameObjectWithTag with name Player
		player = GameObject.FindGameObjectWithTag("Player").transform;
		// spotlight.spotAngle
		viewAngle = spotlight.spotAngle;
		// spotlight.color
		originalSpotlightColour = spotlight.color;
		// Intialize vector/Array
		Vector3[] waypoints = new Vector3[pathHolder.childCount]; 
		for (int i = 0; i < waypoints.Length; i++) { //Loop
			waypoints [i] = pathHolder.GetChild (i).position; 
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y, waypoints [i].z);
		}
		// StartCoroutine FollowPath
		StartCoroutine (FollowPath (waypoints));

	}

	// Update Function
	void Update() {
		
		if (CanSeePlayer ()) {
			// 
			playerVisibleTimer += Time.deltaTime;
		} else {
			playerVisibleTimer -= Time.deltaTime;
		}
		// value between 0 and timeToSpotPlayer
		playerVisibleTimer = Mathf.Clamp (playerVisibleTimer, 0, timeToSpotPlayer); 
		// color between originalSpotlightColour and Color.red (0,1)
		spotlight.color = Color.Lerp (originalSpotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer); 

		if (playerVisibleTimer >= timeToSpotPlayer) 
		{
			// checking delegate is null or not
			if (OnGuardHasSpottedPlayer != null) {
				OnGuardHasSpottedPlayer ();
			}
		}
	}

	// CanSeePlayer Function
	bool CanSeePlayer() {
		// Condition
		if (Vector3.Distance(transform.position,player.position) < viewDistance) {
			Vector3 dirToPlayer = (player.position - transform.position).normalized; // Only Direction
			float angleBetweenGuardAndPlayer = Vector3.Angle (transform.forward, dirToPlayer); // angleBetweenGuardAndPlayer
			if (angleBetweenGuardAndPlayer < viewAngle / 2f) 
			{ // Condition
				if (!Physics.Linecast (transform.position, player.position, viewMask)) { // Linecast
					return true;
				}
			}
		}
		return false;
	}

	// Coroutine followpath
	IEnumerator FollowPath(Vector3[] waypoints) {
		transform.position = waypoints [0]; // first waypoints

		int targetWaypointIndex = 1; // next waypoints
		Vector3 targetWaypoint = waypoints [targetWaypointIndex]; // next waypoints 
		transform.LookAt (targetWaypoint); // look at targetWaypoint 

		while (true) {
			transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, speed * Time.deltaTime); // move towards with that speed
			if (transform.position == targetWaypoint) { // checking 
				targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length; // for all waypoints from last waypoint to first waypoint
				targetWaypoint = waypoints [targetWaypointIndex]; // new waypoint
				yield return new WaitForSeconds (waitTime);  
				yield return StartCoroutine (TurnToFace (targetWaypoint)); // starting coroutine of faceturn
			}
			yield return null;
		}
	}

	// couroutine To turn face at turn speed of 90 deg
	IEnumerator TurnToFace(Vector3 lookTarget) {
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized; // for direction only
		float targetAngle = 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg; // targetangle from radian to degree

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f) { // DeltaAngle should be  > 0.0.5f
			float angle = Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime); // move angle towards target
			transform.eulerAngles = Vector3.up * angle; // transforming angle in eulerAngle represents rotation in world space
			yield return null; 
		}
	}

	// function for EditorGizmos
	void OnDrawGizmos() {
		// startPosition and previousPosition are same
		Vector3 startPosition = pathHolder.GetChild (0).position;
		Vector3 previousPosition = startPosition;

		// Loop for all waypoints
		foreach (Transform waypoint in pathHolder) {
			// DrawSphere at the waypoints position
			Gizmos.DrawSphere (waypoint.position, .3f);
			// DrawLine at the between the waypoints
			Gizmos.DrawLine (previousPosition, waypoint.position);
			// for last waypoint position
			previousPosition = waypoint.position;
		}
		// DrawSphere at the last waypoint position
		Gizmos.DrawLine (previousPosition, startPosition);
		// Gizmos color is red
		Gizmos.color = Color.red;
		Gizmos.DrawRay (transform.position, transform.forward * viewDistance);
	}

}
