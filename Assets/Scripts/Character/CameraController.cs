using UnityEngine;

class CameraController {
    float verticalLookRotation;
	
    Transform cameraTransform;

    public CameraController(Transform _cameraTransform) {
        verticalLookRotation = 0;
        cameraTransform = _cameraTransform;
    }

    public void MoveCamera(float amount) {
        verticalLookRotation += amount;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation,-90,90);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
}