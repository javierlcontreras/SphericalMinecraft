using UnityEngine;

[RequireComponent (typeof(Transform))]
public class ControllerSettings : MonoBehaviour {
    // public vars
	public float mouseSensitivityX = 1;
	public float mouseSensitivityY = 1;
	public float walkSpeed = 6;
	public float jumpForce = 220;
	public float flySpeed = 6;
	public LayerMask groundedMask;

	private Transform cameraTransform;
	public Transform CameraTransform => cameraTransform;
	private Transform characterTransform;
	public Transform CharacterTransform => characterTransform;
    
    public Vector3 lowPoint = new Vector3(0, -0.25f, 0);
	public float skinWidth = 0.25f;

	public Vector3 characterShape = new Vector3(0.6f, 1.8f, 0.4f)*0.8f;

	public float reach = 5;

	void Awake() {	
		cameraTransform = Camera.main.transform;
		characterTransform = GetComponent<Transform> ();
	}
}