using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    public float gravitationalConstant;
    private CelestialBody earth;
    private CelestialBody moon;
    private void Awake() {
        earth = GameObject.Find("Earth").GetComponent<CelestialBody>();
        moon = GameObject.Find("Moon").GetComponent<CelestialBody>();
    }

    void FixedUpdate()
    {
        Vector3 direction = (earth.GetPosition() - moon.GetPosition());
        float distance = direction.magnitude;
        float strength = gravitationalConstant * earth.GetMass()*moon.GetMass() /  distance / distance;
        //earth.UpdateVelocity(Time.fixedDeltaTime, -direction.normalized * strength);
        moon.UpdateVelocity(Time.fixedDeltaTime, direction.normalized * strength);
        moon.UpdatePosition(Time.fixedDeltaTime);

        earth.UpdateRotation(Time.fixedDeltaTime);
        moon.UpdateRotation(Time.fixedDeltaTime);
    }
}
