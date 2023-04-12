using UnityEngine;

class TangencialMovementController {
    Vector3 moveAmount;
	Vector3 smoothMoveVelocity;
	float walkSpeed;

    public TangencialMovementController(float _walkSpeed) {
        walkSpeed = _walkSpeed;
        moveAmount = Vector3.zero;
    }

    public Vector3 AmountToMoveWithTarget(float inputX, float inputY) {
        Vector3 moveDir = new Vector3(inputX,0, inputY).normalized;
		Vector3 targetMoveAmount = moveDir * walkSpeed;
		//moveAmount = Vector3.SmoothDamp(moveAmount,targetMoveAmount,ref smoothMoveVelocity,.15f);
        return targetMoveAmount;
    }
}