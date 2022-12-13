using UnityEngine;

class NormalMovementController {
    //bool grounded;
	//bool jumping;
	float verticalVelocity;
	float shiftAndJumpVelocities = 6f;
    Transform character;
    ControllerSettings settings;
	private float feetSkinWidth;
	private Vector3 feetPoint;

    private LayerMask groundedMask;
    public NormalMovementController(ControllerSettings _settings) {	
        verticalVelocity = 0;
        settings = _settings;
        groundedMask = settings.groundedMask;
        feetSkinWidth = settings.feetSkinWidth;
        feetPoint = settings.feetPoint;
        character = settings.CharacterTransform;
    }

    public float VerticalVelocity(bool wantToJump, bool wantToShift) {
		float addedVel = 0;
		if (wantToJump) addedVel += shiftAndJumpVelocities;
		if (wantToShift) addedVel -= shiftAndJumpVelocities;
		
        if (!Grounded()) {
            verticalVelocity -= 9.8f*Time.fixedDeltaTime;
        } 
        else {
            verticalVelocity = 0;
        }
        return verticalVelocity + addedVel;
    }

    public bool Grounded() {
		Ray rayDown = new Ray(character.TransformPoint(feetPoint), -character.up);
		RaycastHit hit;
		if (Physics.Raycast(rayDown, out hit, feetSkinWidth, groundedMask)) return true;
		return false;
	
    }
}