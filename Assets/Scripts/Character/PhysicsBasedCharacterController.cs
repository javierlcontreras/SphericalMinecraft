using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBasedCharacterController : MonoBehaviour
{
    public float gravityStrength = 10;
    public float movementSpeed = 5;
    public float movementAcc = 2;
    public float dampingFactor = 0.1f;
    public float jumpStrength = 20;
    
    Rigidbody playerRigidBody;
    Transform playerTransform;
    public Transform cameraTransform;

    private Vector3 radialDirection;

    void Start()
    {
        playerTransform = GetComponent<Transform>();   
        playerRigidBody = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        radialDirection = Vector3.Normalize(playerTransform.position);
        playerTransform.rotation = RadialCharacterOrientation(); 

        CameraControl();
        MovementControl();

        playerRigidBody.AddForce(-radialDirection*gravityStrength);  
    }

    void CameraControl() {
        Vector3 mouse = Input.mousePosition;
        float width = mouse.x;
        float height = mouse.y;

        

    }

    void MovementControl() {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey("w")) dir += Vector3.forward;
        if (Input.GetKey("s")) dir += Vector3.back;
        if (Input.GetKey("a")) dir += Vector3.left;
        if (Input.GetKey("d")) dir += Vector3.right;
        if (Input.GetKey("space")) {
            Vector3 globalUp = playerTransform.TransformDirection(Vector3.up);
            playerRigidBody.AddForce(globalUp*jumpStrength);
        }
        //if (Input.GetKey(KeyCode.LeftShift)) dir += Vector3.down;
        dir = Vector3.Normalize(dir);

        Vector3 globalDir = playerTransform.TransformDirection(dir);

        playerRigidBody.AddForce(globalDir*movementAcc);
        
        playerRigidBody.AddForce(-playerRigidBody.velocity*dampingFactor);
    }

    Quaternion RadialCharacterOrientation() {
        Vector3 globalForward = playerTransform.TransformDirection(Vector3.forward);
        Vector3 properUp = radialDirection;
        Vector3 properForward = Vector3.Cross(Vector3.Cross(properUp, globalForward), properUp);
        Quaternion playerOrientation = Quaternion.LookRotation(properForward, properUp);
        
        return playerOrientation;
    }
}
