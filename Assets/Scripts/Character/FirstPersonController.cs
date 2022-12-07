using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(ControllerSettings))]
public class FirstPersonController : MonoBehaviour {

	private ControllerSettings settings;
	
	private TangencialMovementController tangencialController;
	private NormalMovementController normalController;
	private CameraController cameraController;
	
	private Transform characterTransform;
	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;	
		settings = GetComponent<ControllerSettings>();

		tangencialController = new TangencialMovementController(settings.walkSpeed);
		normalController = new NormalMovementController(settings);
		cameraController = new CameraController(settings.CameraTransform);
        characterTransform = settings.CharacterTransform;	
	}
	void FixedUpdate() {
        characterTransform.rotation = settings.RadialCharacterOrientation();
		//float scalingFactor = ScalePlayerWithHeight();
		//characterTransform.localScale = settings.characterShape * scalingFactor;
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		bool wantToJump = Input.GetButton("Jump");
		bool wantToShift = Input.GetButton("Shift");

		// Set characterTransform radially
		characterTransform.Rotate(Vector3.up * mouseX * settings.mouseSensitivityX);
		// Move camera by input
		cameraController.MoveCamera(mouseY * settings.mouseSensitivityY);
		
		Vector3 moveAmount = tangencialController.AmountToMoveWithTarget(inputX, inputY);
		float verticalVelocity = normalController.VerticalVelocity(wantToJump, wantToShift);

		Vector3 finalMove = characterTransform.TransformDirection(moveAmount + Vector3.up*verticalVelocity) * Time.fixedDeltaTime;
		
		Vector3 radialDirection = characterTransform.position.normalized;
		Vector3 p1 = characterTransform.TransformPoint(settings.lowPoint);
		int tries = 10;
		while (tries > 0) {
			tries--;
			RaycastHit hit;
			if (Physics.Raycast(p1, finalMove, out hit, finalMove.magnitude+settings.skinWidth, settings.groundedMask)) {
				finalMove -= Vector3.Dot(finalMove, hit.normal)*hit.normal;
			}
			else {
				break;
			}
		}

		characterTransform.Translate(characterTransform.InverseTransformDirection(finalMove));
	}
}