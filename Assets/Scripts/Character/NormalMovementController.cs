using UnityEngine;

class NormalMovementController {
    //bool grounded;
	//bool jumping;
	float verticalVelocity;
	float shiftAndJumpVelocities = 6f;
    Transform character;
	private float feetSkinWidth = 0.5f;
	private Vector3 feetPoint = new Vector3(0, -0.25f, 0);

    private LayerMask groundedMask;
    public NormalMovementController(ControllerSettings settings) {	
        verticalVelocity = 0;
        groundedMask = settings.groundedMask;
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