using Unity.Netcode;
using UnityEngine;

[RequireComponent (typeof(ChunkLoader))]
public class ControllerSettings : NetworkBehaviour {
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

	public float gravitationalPullDown;
	public float gravitationalPullUp;
	public float jumpStrength;
	public float jumpTimer;

	public Vector3 initialPosition;
	
	private ChunkLoader chunkLoader;
	public void Init(ChunkLoader _chunkLoader)
	{
		chunkLoader = _chunkLoader;
	}

    public Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = CharacterTransform.TransformDirection(Vector3.forward);
		
        GameObject currentPlanet = chunkLoader.GetCurrentPlanet();
        Vector3 currentPlanetBodyPosition = Vector3.zero;
	    if (currentPlanet != null)
	    {
		    currentPlanetBodyPosition = currentPlanet.GetComponent<CelestialBody>().GetPosition();
	    }
	    Vector3 properUp = (CharacterTransform.position - currentPlanetBodyPosition).normalized;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}