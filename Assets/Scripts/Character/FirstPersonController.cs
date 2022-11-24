using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(Collider))]
public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;
	
	Transform cameraTransform;
	Transform character;
	
	private TangencialMovementController tangencialController;
	private NormalMovementController normalController;
	private CameraController cameraController;

	private HashSet<Collision> blockedDirections;

	void Awake() {
		blockedDirections = new HashSet<Collision> ();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		cameraTransform = Camera.main.transform;
		
		character = GetComponent<Transform> ();

		tangencialController = new TangencialMovementController(walkSpeed);
		float initYPosition = character.position.magnitude;
		normalController = new NormalMovementController(initYPosition, character, groundedMask);
		cameraController = new CameraController(cameraTransform);
	}
	
	void FixedUpdate() {
        character.rotation = RadialCharacterOrientation();
		
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		bool wantToJump = Input.GetButton("Jump");
		bool wantToShift = Input.GetButton("Shift");

		// Set character radially
		character.Rotate(Vector3.up * mouseX * mouseSensitivityX);
		// Move camera by input
		cameraController.MoveCamera(mouseY * mouseSensitivityY);
		
		// Calculate movement Tangencial
		Vector3 moveAmount = tangencialController.AmountToMoveWithTarget(inputX, inputY);
		//Vector3 move = character.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		// Calculate movement Vertical
		float verticalVelocity = normalController.VerticalVelocity(wantToJump, wantToShift);

		//Debug.Log(verticalVelocity + " " + moveAmount + normalController.Grounded());
		//Debug.Log(RadialCharacterOrientation());
		
		Vector3 finalMove = moveAmount + Vector3.up*verticalVelocity;
		
		//Debug.Log("Prohibited");
		foreach (Collision collision in blockedDirections) {
			foreach (ContactPoint contact in collision.contacts) {
				Vector3 dir = character.InverseTransformDirection(contact.normal);
				float amountInDirection = Vector3.Dot(finalMove, dir);
				if (amountInDirection < 0) {
					finalMove -= amountInDirection*dir;
				}
			}
		}
		character.Translate(finalMove * Time.fixedDeltaTime);
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = character.TransformDirection(Vector3.forward);
        Vector3 properUp = character.position.normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }

	void OnCollisionEnter(Collision collision)
    {
		blockedDirections.Add(collision);
    }

	void OnCollisionStay(Collision collision) {
		blockedDirections.Add(collision);
	}

	void OnCollisionExit(Collision collision) {
		blockedDirections.Remove(collision);
	}
}