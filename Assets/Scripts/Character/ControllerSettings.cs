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

	public Transform CameraTransform;
	public Transform CharacterTransform;
	
    public Vector3 lowPoint = new Vector3(0, -0.25f, 0);
	public float skinWidth = 0.25f;

	public Vector3 characterShape = new Vector3(0.6f, 1.8f, 0.4f)*0.8f;

	public float reach = 5;

	public Vector3 feetPoint = new Vector3(0, -0.25f, 0);
	public float feetSkinWidth = 0.5f;


    public Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = CharacterTransform.TransformDirection(Vector3.forward);
		
        Vector3 properUp = (CharacterTransform.position - TerrainManager.instance.GetCurrentPlanet().GetPosition()).normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}