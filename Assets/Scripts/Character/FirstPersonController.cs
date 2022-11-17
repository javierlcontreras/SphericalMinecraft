using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GravityBody))]
public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;
	
	// System vars
	bool grounded;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody playerRigidbody;
	
    private Vector3 radialDirection;
	
	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cameraTransform = Camera.main.transform;
		playerRigidbody = GetComponent<Rigidbody> ();
	}
	
	void Update() {
		radialDirection = Vector3.Normalize(transform.position);

        transform.rotation = RadialCharacterOrientation();
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-80,80);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
		
		// Calculate movement:
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		
		Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);

	}

	bool jumpHalted() {
		float verticalVel = Vector3.Dot(playerRigidbody.velocity, radialDirection);
		//Debug.Log(verticalVel);
		return verticalVel < 0.000001f;
	}
	
	void FixSlopeSpeeding() {
		// TODO: fix this idea to fix slope shooting up. Basically steal control from physics engine
		float vx = playerRigidbody.velocity.x;
		float currentVerticalSpeed = playerRigidbody.velocity.y;
		float vz = playerRigidbody.velocity.z;
	
		if(grounded)
		{
			if(currentVerticalSpeed < 0f)
				currentVerticalSpeed = 0f;
		}
		playerRigidbody.velocity = new Vector3(vx, currentVerticalSpeed, vz);
	}

	void FixedUpdate() {
		// FixSlopeSpeeding();
		// Jump
		if (Input.GetButton("Jump")) {
			if (grounded && jumpHalted()) {
				playerRigidbody.AddForce(transform.up * jumpForce);
			}
		}

		grounded = false;
		Vector3[] adds = new Vector3[] {
			transform.right + transform.forward,
			-transform.right + transform.forward,
			transform.right - transform.forward,
			-transform.right - transform.forward
		};
		for (int ray = 0; ray < 4; ray++) {
			Vector3 add = adds[ray] * 0.1f; 
			Ray rayDown = new Ray(transform.position + add, -transform.up);
			RaycastHit hit;

			if (Physics.Raycast(rayDown, out hit, 1 + .5f, groundedMask)) grounded = true;
		}
		

		// Apply movement to playerRigidbody
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		playerRigidbody.MovePosition(playerRigidbody.position + localMove);
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = transform.TransformDirection(Vector3.forward);
        Vector3 properUp = radialDirection;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}