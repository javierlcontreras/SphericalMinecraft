using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;

[RequireComponent (typeof(ControllerSettings))]
public class FirstPersonController : NetworkBehaviour
{

	private ControllerSettings settings;
	
	private TangencialMovementController tangencialController;
	private NormalMovementController normalController;
	private CameraController cameraController;

	private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(Vector3.zero);
	private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(Quaternion.identity);
	private Transform characterTransform;
	private bool isFlying;
	
	public void Init(ControllerSettings _settings)
	{
		settings = _settings;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		tangencialController = new TangencialMovementController(settings.walkSpeed);
		normalController = new NormalMovementController(settings);
		cameraController = new CameraController(settings.CameraTransform);
        characterTransform = settings.CharacterTransform;	
		isFlying = false;

		characterTransform.position = settings.initialPosition;
		position.Value = settings.initialPosition;
	}
	
	public void MyFixedUpdate()
	{
		transform.position = position.Value;
		transform.rotation = rotation.Value;
		if (Input.GetKeyDown(KeyCode.C)) {
			if (isFlying) {
				normalController.SetVerticalVelocity(0);
			}
			isFlying  = !isFlying ;
		}
		RotateMeServerRpc(settings.RadialCharacterOrientation());
		//float scalingFactor = ScalePlayerWithHeight();
		//characterTransform.localScale = settings.characterShape * scalingFactor;
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		bool wantToJump = Input.GetButton("Jump");
		bool wantToShift = Input.GetButton("Shift");

		// Set characterTransform radially
		characterTransform.Rotate(Vector3.up * (mouseX * settings.mouseSensitivityX));
		// Move camera by input
		cameraController.MoveCamera(mouseY * settings.mouseSensitivityY);
		
		Vector3 moveAmount = tangencialController.AmountToMoveWithTarget(inputX, inputY);
		float verticalVelocity = normalController.UpdateVerticalVelocity(wantToJump, wantToShift, Time.fixedDeltaTime);
		if (isFlying) {
			verticalVelocity = 0;
			if (wantToJump) verticalVelocity += settings.flySpeed;
			if (wantToShift) verticalVelocity -= settings.flySpeed;
		}
		Vector3 finalMove = characterTransform.TransformDirection(moveAmount + Vector3.up*verticalVelocity) * Time.fixedDeltaTime;
		
		Vector3 radialDirection = characterTransform.position.normalized;
		Vector3 p1 = characterTransform.TransformPoint(settings.lowPoint);
		Vector3 p2 = characterTransform.TransformPoint(settings.midPoint);
		Vector3 p3 = characterTransform.TransformPoint(settings.highPoint);
		int tries = 10;
		while (tries > 0) {
			tries--;
			RaycastHit hit;
			if (Physics.Raycast(p1, finalMove, out hit, finalMove.magnitude+settings.skinWidth, settings.groundedMask)) {
				finalMove -= Vector3.Dot(finalMove, hit.normal)*hit.normal;
			}
			else if (Physics.Raycast(p2, finalMove, out hit, finalMove.magnitude+settings.skinWidth, settings.groundedMask)) {
				finalMove -= Vector3.Dot(finalMove, hit.normal)*hit.normal;
			}
			else if (Physics.Raycast(p3, finalMove, out hit, finalMove.magnitude+settings.skinWidth, settings.groundedMask)) {
				finalMove -= Vector3.Dot(finalMove, hit.normal)*hit.normal;
			}
			else {
				break;
			}
		}

		Vector3 finalMoveInGlobal = characterTransform.InverseTransformDirection(finalMove);
		float dx = finalMoveInGlobal.x;
		float dy = finalMoveInGlobal.y;
		float dz = finalMoveInGlobal.z;
		if (dx != 0 || dy != 0 || dz != 0)
		{
			MoveMeServerRpc(new Vector3(dx, dy, dz));
		}
	}
	
	[ServerRpc]
	public void MoveMeServerRpc(Vector3 diff, ServerRpcParams serverRpcParams = default)
	{
		position.Value += diff;
	}
	
	[ServerRpc]
	public void RotateMeServerRpc(Quaternion newRotation, ServerRpcParams serverRpcParams = default)
	{
		rotation.Value = newRotation;
	}
	
	public float GetVerticalVelocity() {
		return normalController.GetVerticalVelocity();
	}
	public void SetVerticalVelocity(float verticalVelocity) {
		normalController.SetVerticalVelocity(verticalVelocity);
	}
	public bool GetIsFlying() {
		return isFlying;
	}
	public void SetIsFlying(bool _isFlying) {
		isFlying = _isFlying;
	}
}