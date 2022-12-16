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
        transform.Rotate(transform.up*90*deltaTime*rotationalSpeed);
    }
    public void UpdateVelocity(float deltaTime, Vector3 force) {
        velocity += deltaTime * (force / mass);
    }
}
