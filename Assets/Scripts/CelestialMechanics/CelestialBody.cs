using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass;
    public float rotationalSpeed;
    public float GetMass() {
        return mass;
    }
    public Vector3 velocity;
    public Vector3 GetPosition() {
        return transform.position;
    }
    public Vector3 GetVelocity() {
        return velocity;
    }
    public void SetVelocity(Vector3 _velocity) {
        velocity = _velocity;
    }
    public Quaternion GetRotation() {
        return transform.rotation;
    }
    public void UpdatePosition(float deltaTime) {
        transform.position += deltaTime*velocity;
    }
    public void UpdateRotation(float deltaTime) {
        transform.Rotate(Vector3.up*90*deltaTime*rotationalSpeed);
    }
    public void UpdateVelocity(float deltaTime, Vector3 force) {
        velocity += deltaTime * (force / mass);
    }
    public void SnapPlayer(Transform player, float deltaTime) {
        Quaternion rotation = Quaternion.Euler(0, 90*deltaTime*rotationalSpeed, 0);
        player.position = transform.TransformPoint(rotation * (transform.InverseTransformPoint(player.position)));
        player.transform.Rotate(player.transform.up*90*deltaTime*rotationalSpeed);
        player.position += deltaTime *  velocity;
    }
    public void LocalToGlobal(Transform obj) {
        obj.position = transform.TransformPoint(obj.position);
        Debug.Log("Planet rotation " + transform.rotation + " Obj rot " + obj.rotation);
        obj.rotation = transform.rotation * obj.rotation;
        Debug.Log("Planet rotation " + transform.rotation + " Obj rot " + obj.rotation);
    }
}
