using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour {
	
	public GravityAttractor attractor;
	Rigidbody objectRigidbody;
	
	void Awake () {
		//planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
		objectRigidbody = GetComponent<Rigidbody> ();

		// Disable objectRigidbody gravity and rotation as this is simulated in GravityAttractor script
		objectRigidbody.useGravity = false;
		objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	void FixedUpdate () {
		// Allow this body to be influenced by planet's gravity
		attractor.AttractInertBody(objectRigidbody);
	}
}