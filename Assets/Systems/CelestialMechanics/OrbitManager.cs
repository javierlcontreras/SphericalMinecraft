using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    private CelestialBody earth;
    private CelestialBody moon;
    private void Awake() {
        earth = GameObject.Find("Earth").GetComponent<CelestialBody>();
        moon = GameObject.Find("Moon").GetComponent<CelestialBody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = (earth.GetPosition() - moon.GetPosition());
        float distance = direction.magnitude;
        float strength = earth.GetMass()*moon.GetMass() /  distance / distance;
        // earth.UpdateVelocity(Time.fixedDeltaTime, -direction.normalized * strength);
        // moon.UpdateVelocity(Time.fixedDeltaTime, direction.normalized * strength);

        // earth.UpdatePosition(Time.fixedDeltaTime);
        // moon.UpdatePosition(Time.fixedDeltaTime);
    }
}
