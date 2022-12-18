using UnityEngine;

[RequireComponent (typeof(ChunkLoader))]
public class ControllerSettings : MonoBehaviour {
    // public vars
	public float mouseSensitivityX;
	public float mouseSensitivityY;
	public float walkSpeed;
	public float jumpForce;
	public float flySpeed;
	public LayerMask groundedMask;

	public Transform CameraTransform;
	public Transform CharacterTransform;
	
    public Vector3 lowPoint;
    public Vector3 midPoint;
    public Vector3 highPoint;
	public float skinWidth;
	public float feetWidth;

	public float reach;
	public float maxVerticalVelocity;

	public float gravitationalPull;
	public float jumpStrength;
	public float jumpTimer;

	private ChunkLoader chunkLoader;
	public void Awake() {
		chunkLoader = gameObject.GetComponent<ChunkLoader>();
	}

    public Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = CharacterTransform.TransformDirection(Vector3.forward);
		
		CelestialBody currentPlanetBody = chunkLoader.GetCurrentPlanet().GetComponent<CelestialBody>();
        Vector3 properUp = (CharacterTransform.position - currentPlanetBody.GetPosition()).normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}