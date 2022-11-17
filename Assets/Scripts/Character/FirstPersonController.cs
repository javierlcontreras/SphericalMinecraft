using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;
	
	// System vars
	bool grounded;
	bool jumping;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float verticalLookRotation;
	Transform cameraTransform;
	Rigidbody playerRigidbody;
	
    private Vector3 radialDirection;
	float verticalVelocity;
	void Awake() {
		verticalVelocity = 0;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		cameraTransform = Camera.main.transform;
		playerRigidbody = GetComponent<Rigidbody> ();
		playerRigidbody.useGravity = false;
		playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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
		if (verticalVelocity < 0.000001f) {
			jumping = false;
			return true;
		}
		return false;
	}
	
	void ClampYVelocity() {
		// TODO: fix this idea to fix slope shooting up. Basically steal control from physics engine
		float currentVerticalSpeed = Vector3.Dot(playerRigidbody.velocity, radialDirection);	
		playerRigidbody.velocity -= currentVerticalSpeed*radialDirection;
		playerRigidbody.velocity += verticalVelocity*radialDirection;
	}

	void FixedUpdate() {
		// FixSlopeSpeeding();
		// Jump
		if (Input.GetButton("Jump")) {
			if (grounded && jumpHalted()) {
				playerRigidbody.AddForce(transform.up * jumpForce);
				jumping = true;
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
		if (grounded == false) {
			verticalVelocity -= 9.8f*Time.fixedDeltaTime;
		}
		else {
			if (jumping == false) verticalVelocity = 0;
		}
		

		// Apply movement to playerRigidbody
		Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		playerRigidbody.MovePosition(playerRigidbody.position + localMove);

		ClampYVelocity();
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = transform.TransformDirection(Vector3.forward);
        Vector3 properUp = radialDirection;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}