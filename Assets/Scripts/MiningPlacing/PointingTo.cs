using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ControllerSettings))]
public class PointingTo : MonoBehaviour
{
    ControllerSettings settings;
    public Vector3 GlobalPointingPoint(){
        RaycastHit hit;
        Physics.Raycast(settings.CameraTransform.position, settings.CameraTransform.forward, out hit, settings.reach, settings.groundedMask);
        return hit.point;
    }

    public Block PointingToBlock() {
        Vector3 pointing = GlobalPointingPoint();
        return null;
    }

    void Start() {
        settings = GetComponent<ControllerSettings>();
    }

    void Update() {
        PointingToBlock();
    }

}
