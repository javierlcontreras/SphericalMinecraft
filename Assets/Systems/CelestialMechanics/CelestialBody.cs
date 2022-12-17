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
}
