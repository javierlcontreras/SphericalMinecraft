using UnityEngine;

class NormalMovementController {
    //bool grounded;
	//bool jumping;
	//float verticalPosition;
	float verticalVelocity;
    CharacterController body;
	/*
    void Jump() {
        if (Input.GetButton("Jump")) {
			if (grounded && jumpHalted()) {
				playerRigidbody.AddForce(transform.up * jumpForce);
				jumping = true;
			}
		}
    }
    */
    private LayerMask groundedMask;
    public NormalMovementController(float initVerticalPosition, CharacterController _body, LayerMask _groundedMask) {
        //verticalPosition = initVerticalPosition;
        verticalVelocity = 0;
        groundedMask = _groundedMask;
        body = _body;
    }

    public float VerticalVelocity() {
        if (!Grounded()) {
            verticalVelocity -= 9.8f*Time.fixedDeltaTime;
        } 
        else {
            verticalVelocity = 0;
        }
        return verticalVelocity;
    }

    bool Grounded() {
        bool grounded = false;
        Transform transform = body.transform;
		Vector3[] adds = new Vector3[] {
			transform.right + transform.forward,
			-transform.right + transform.forward,
			transform.right - transform.forward,
			-transform.right - transform.forward
		};
		for (int ray = 0; ray < 4; ray++) {
			Vector3 add = adds[ray] * 0.1f; 
			Ray rayDown = new Ray(transform.position + add, -transform.up);
			RaycastHit hit;

			if (Physics.Raycast(rayDown, out hit, 1 + .5f, groundedMask)) grounded = true;
		}
        return grounded;
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