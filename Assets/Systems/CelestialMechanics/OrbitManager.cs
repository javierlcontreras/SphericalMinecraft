using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitManager : MonoBehaviour
{
    public float gravitationalConstant;
    private CelestialBody earth;
    private CelestialBody moon;

    void FixedUpdate()
    {
        earth = GameObject.Find("Earth").GetComponent<CelestialBody>();
        moon = GameObject.Find("Moon").GetComponent<CelestialBody>();

        Vector3 direction = (earth.GetPosition() - moon.GetPosition());
        float distance = direction.magnitude;
        float strength = gravitationalConstant * earth.GetMass()*moon.GetMass() /  distance / distance;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            ChunkLoader chunkLoader = player.GetComponent<ChunkLoader>();
            chunkLoader.GetCurrentPlanet().GetComponent<CelestialBody>().SnapPlayer(player.transform, Time.fixedDeltaTime);
        }

        //earth.UpdateVelocity(Time.fixedDeltaTime, -direction.normalized * strength);
        moon.UpdateRotation(Time.fixedDeltaTime);
        moon.UpdatePosition(Time.fixedDeltaTime);
        moon.UpdateVelocity(Time.fixedDeltaTime, direction.normalized * strength);

        earth.UpdateRotation(Time.fixedDeltaTime);
    }
}
