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
	Transform characterTransform;
	
	private TangencialMovementController tangencialController;
	private NormalMovementController normalController;
	private CameraController cameraController;
	private Vector3 lowPoint = new Vector3(0, -0.25f, 0);
	private float skinWidth = 0.25f;

	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		cameraTransform = Camera.main.transform;
		
		characterTransform = GetComponent<Transform> ();

		tangencialController = new TangencialMovementController(walkSpeed);
		float initYPosition = characterTransform.position.magnitude;
		normalController = new NormalMovementController(initYPosition, characterTransform, groundedMask);
		cameraController = new CameraController(cameraTransform);
	}
	
	void FixedUpdate() {
        characterTransform.rotation = RadialCharacterOrientation();
		
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		bool wantToJump = Input.GetButton("Jump");
		bool wantToShift = Input.GetButton("Shift");

		// Set characterTransform radially
		characterTransform.Rotate(Vector3.up * mouseX * mouseSensitivityX);
		// Move camera by input
		cameraController.MoveCamera(mouseY * mouseSensitivityY);
		
		Vector3 moveAmount = tangencialController.AmountToMoveWithTarget(inputX, inputY);
		float verticalVelocity = normalController.VerticalVelocity(wantToJump, wantToShift);

		Vector3 finalMove = characterTransform.TransformDirection(moveAmount + Vector3.up*verticalVelocity) * Time.fixedDeltaTime;
		
		Vector3 radialDirection = characterTransform.position.normalized;
		Vector3 p1 = characterTransform.TransformPoint(lowPoint);
		int tries = 10;
		while (tries > 0) {
			tries--;
			RaycastHit hit;
			if (Physics.Raycast(p1, finalMove, out hit, finalMove.magnitude+skinWidth, groundedMask)) {
				Debug.Log("Collision!");
				finalMove -= Vector3.Dot(finalMove, hit.normal)*hit.normal;
			}
			else {
				break;
			}
		}

		characterTransform.Translate(characterTransform.InverseTransformDirection(finalMove));
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = characterTransform.TransformDirection(Vector3.forward);
        Vector3 properUp = characterTransform.position.normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}