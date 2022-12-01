using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(ControllerSettings))]
public class CreativeModeController : MonoBehaviour {

	private ControllerSettings settings;
	
	private CameraController cameraController;
	
	private Transform characterTransform;
	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		settings = GetComponent<ControllerSettings>();
	}
    void Start() {
        characterTransform = settings.CharacterTransform;		
		cameraController = new CameraController(settings.CameraTransform);
    }

	void FixedUpdate() {
        characterTransform.rotation = RadialCharacterOrientation();
		float scalingFactor = ScalePlayerWithHeight();
		characterTransform.localScale = new Vector3(0.6f*scalingFactor, 1.8f*scalingFactor, 0.4f*scalingFactor);
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
		
		Vector3 finalMove = (new Vector3(inputX, 0, inputY)) * settings.flySpeed;
        if (wantToJump) {
            finalMove.y += settings.flySpeed;
        }
        if (wantToShift) {
            finalMove.y -= settings.flySpeed;
        }
		characterTransform.Translate(finalMove);
	}

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = characterTransform.TransformDirection(Vector3.forward);
        Vector3 properUp = characterTransform.position.normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }

	float ScalePlayerWithHeight() {
		return characterTransform.position.magnitude / TerrainManager.instance.PlanetRadius;
	}
}