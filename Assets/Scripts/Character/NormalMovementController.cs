using UnityEngine;

class NormalMovementController {
    //bool grounded;
	//bool jumping;
	private float verticalVelocity;
    
    private bool jumping = false;
    private float timeSinceJump = 0;
	Transform character;
    ControllerSettings settings;
	
    public NormalMovementController(ControllerSettings _settings) {	
        verticalVelocity = 0;
        settings = _settings;
        character = settings.CharacterTransform;
    }

	public float GetVerticalVelocity() {
		return verticalVelocity;
	}
    public void SetVerticalVelocity(float _verticalVelocity) {
        verticalVelocity = _verticalVelocity;
    }

    public float UpdateVerticalVelocity(bool wantToJump, bool wantToShift, float deltaTime) {
        timeSinceJump += deltaTime;
        if (timeSinceJump > settings.jumpTimer) {
            jumping = false;
        }
        

        if (!Grounded()) {
            float gravity = settings.gravitationalPullDown;
            if (verticalVelocity > 0) gravity = settings.gravitationalPullUp;
            // TODO: this  modification of velocity should be done server side, this is why client just falls do the void
            verticalVelocity -= gravity*deltaTime;
        } 
        else {
            if (timeSinceJump > settings.jumpTimer) verticalVelocity = 0;
            if (wantToJump && !jumping) {
                verticalVelocity += settings.jumpStrength;
                jumping = true;
                timeSinceJump = 0;
            }
        }
        if (verticalVelocity > settings.maxVerticalVelocity) verticalVelocity = settings.maxVerticalVelocity;
        if (verticalVelocity < -settings.maxVerticalVelocity) verticalVelocity = -settings.maxVerticalVelocity;
        return verticalVelocity;
    }

    public bool Grounded() {
		Ray rayDown = new Ray(character.TransformPoint(settings.lowPoint), -character.up);
		Debug.DrawRay(character.TransformPoint(settings.lowPoint), -character.up*settings.feetWidth, Color.red  );
        RaycastHit hit;
        if (Physics.Raycast(rayDown, out hit, settings.feetWidth, settings.groundedMask))
        {
            return true;
        }
		return false;
    }
}