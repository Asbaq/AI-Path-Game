using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {

	// Initialize Action event delegate 
	public event System.Action OnReachedEndOfLevel;

	// Initialize Variable 
	public float moveSpeed = 7;
	public float smoothMoveTime = .1f;
	public float turnSpeed = 8;
	float angle;
	float smoothInputMagnitude;
	float smoothMoveVelocity;
	Vector3 velocity;
	new Rigidbody rigidbody;
	bool disabled;

	// Start Function
	void Start() {
		// Intialize in Rigidbody
		rigidbody = GetComponent<Rigidbody> ();
		// Subscribing method to delegate
		Guard_Movement.OnGuardHasSpottedPlayer += Disable;
	}

	void Update () {
		// InputDirection
		Vector3 inputDirection = Vector3.zero;
		if (!disabled)
		{
			// Only Direction from GetAxisRaw
			inputDirection = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized; 
		}
		// Only Magnitude from GetAxisRaw
		float inputMagnitude = inputDirection.magnitude; 
		// to make control smooth
		smoothInputMagnitude = Mathf.SmoothDamp (smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime); 
		// Angle between x direction and z direction (Randian to Degree) 
		float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg; 
		//  Linear Interpolation between angle to targetangle - is a mathematical function in Unity that returns a value between two others at a point on a linear scale.
		angle = Mathf.LerpAngle (angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude); 
		// velocity of player
		velocity = transform.forward * moveSpeed * smoothInputMagnitude; 
	}

	// OnTrigger
	void OnTriggerEnter(Collider hitCollider)
	 {
		// condition
		if (hitCollider.tag == "Finish") { 
			// calling disable function
			Disable ();
			// checking delegate is null or not
			if (OnReachedEndOfLevel != null) {
				OnReachedEndOfLevel (); 
			}
		}
	}

	// making disable variable true
	void Disable() {
		disabled = true;
	}

	void FixedUpdate() {
		// moveRotation in fixed framerate
		rigidbody.MoveRotation (Quaternion.Euler (Vector3.up * angle));
		// moveRotation in fixed framerate
		rigidbody.MovePosition (rigidbody.position + velocity * Time.deltaTime);
	}

	void OnDestroy() {
		// Unsubscribing method to delegate
		Guard_Movement.OnGuardHasSpottedPlayer -= Disable;
	}
}
