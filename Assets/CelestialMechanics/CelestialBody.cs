using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass;
    public float GetMass() {
        return mass;
    }
    public Vector3 velocity;
    public Vector3 GetPosition() {
        return transform.position;
    }

    public void UpdatePosition(float deltaTime) {
        transform.position += deltaTime*velocity;
        transform.Rotate(transform.position.normalized*90*deltaTime);
    }
    public void UpdateVelocity(float deltaTime, Vector3 force) {
        velocity += deltaTime * (force / mass);
    }
}
