using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
	
	// public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public LayerMask groundedMask;
	
	Transform cameraTransform;
	CharacterController characterController;
	
	private TangencialMovementController tangencialController;
	private NormalMovementController normalController;
	private CameraController cameraController;

	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		
		cameraTransform = Camera.main.transform;
		

		characterController = GetComponent<CharacterController> ();

		tangencialController = new TangencialMovementController(walkSpeed);
		float initYPosition = characterController.transform.position.magnitude;
		normalController = new NormalMovementController(initYPosition, characterController, groundedMask);
		cameraController = new CameraController(cameraTransform);
	}
	
	void FixedUpdate() {
        characterController.transform.rotation = RadialCharacterOrientation();
		
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		bool wantToJump = Input.GetButton("Jump");

		// Set character radially
		characterController.transform.Rotate(Vector3.up * mouseX * mouseSensitivityX);
		// Move camera by input
		cameraController.MoveCamera(mouseY * mouseSensitivityY);
		
		// Calculate movement Tangencial
		Vector3 moveAmount = tangencialController.AmountToMoveWithTarget(inputX, inputY);
		Vector3 move = characterController.transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
		// Calculate movement Vertical
		Vector3 radialDirection = transform.position.normalized;
		float verticalVelocity = normalController.VerticalVelocity()*Time.fixedDeltaTime;

		Debug.Log(verticalVelocity + " " + move);
		characterController.Move(move + radialDirection*verticalVelocity);
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = characterController.transform.TransformDirection(Vector3.forward);
        Vector3 properUp = characterController.transform.position.normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}