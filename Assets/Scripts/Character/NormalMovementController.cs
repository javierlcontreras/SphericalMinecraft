using UnityEngine;

class NormalMovementController {
    //bool grounded;
	//bool jumping;
	//float verticalPosition;
	float verticalVelocity;
	float shiftAndJumpVelocities = 6f;
    Transform character;
	private float feetSkinWidth = 0.5f;
	private Vector3 feetPoint = new Vector3(0, -0.25f, 0);

    private LayerMask groundedMask;
    public NormalMovementController(float initVerticalPosition, Transform _character, LayerMask _groundedMask) {
        //verticalPosition = initVerticalPosition;
        verticalVelocity = 0;
        groundedMask = _groundedMask;
        character = _character;
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
		/*Vector3[] adds = new Vector3[] {
			transform.right + transform.forward,
			-transform.right + transform.forward,
			transform.right - transform.forward,
			-transform.right - transform.forward
		};
		for (int ray = 0; ray < 4; ray++) {
			Vector3 add = adds[ray] * 0.1f; 
			Ray rayDown = new Ray(transform.position + add, -transform.up);

		}
        return grounded;
		*/
    }

   /* bool jumpHalted() {
		if (verticalVelocity < 0.000001f) {
			//jumping = false;
			return true;
		}
		return false;
	}
	
	void ClampYVelocity() {
		// TODO: fix this idea to fix slope shooting up. Basically steal control from physics engine
		//float currentVerticalSpeed = Vector3.Dot(playerRigidbody.velocity, radialDirection);	
		//playerRigidbody.velocity -= currentVerticalSpeed*radialDirection;
		//playerRigidbody.velocity += verticalVelocity*radialDirection;
	}*/
}