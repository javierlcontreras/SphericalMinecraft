using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGeneration {
    private const int seed = 0;

    static float interpolate(float a0, float a1, float w) {
        return (a1 - a0)*w + a0;
    }
    
    static float ManhatanNorm(Vector3 v) {
        return Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);
    }

    static Vector3 randomGradient(int ix, int iy, int iz) {
        float theta0 = Random.Range(0f, 2*Mathf.PI);
        float theta1 = Random.Range(-Mathf.PI/2f, Mathf.PI/2f);
        
        float s0 = Mathf.Sin(theta0);
        float s1 = Mathf.Sin(theta1);
        float c0 = Mathf.Cos(theta0);
        float c1 = Mathf.Cos(theta1);
        Vector3 v = new Vector3(s0*s1, s0*c1, c0);
        return v / ManhatanNorm(v) / 3.0f;
    }
    
    private static int hash(int x, int y, int z) {
        return new Vector3(x,y,z).GetHashCode();
    }

    static float dotGridGradient(int ix, int iy, int iz, float x, float y, float z) {
        Random.InitState(hash(ix, iy, iz));
        Vector3 grad = randomGradient(ix, iy, iz);
        float dx = x - ix; 
        float dy = y - iy; 
        float dz = z - iz; 

        return dx*grad.x + dy*grad.y + dz*grad.z;
    }

    public static float Smooth3DNoise(Vector3 point) {
        float x = point.x;
        float y = point.y;
        float z = point.z;
        
        int x0 = (int)x;
        int y0 = (int)y;
        int z0 = (int)z;
        
        int x1 = x0+1;
        int y1 = y0+1;
        int z1 = z0+1;

        float sx = x - x0;
        float sy = y - y0;
        float sz = z - z0;

        float p000 = dotGridGradient(x0, y0, z0, x, y, z);
        float p001 = dotGridGradient(x0, y0, z1, x, y, z);
        float p010 = dotGridGradient(x0, y1, z0, x, y, z);
        float p011 = dotGridGradient(x0, y1, z1, x, y, z);
        float p100 = dotGridGradient(x1, y0, z0, x, y, z);
        float p101 = dotGridGradient(x1, y0, z1, x, y, z);
        float p110 = dotGridGradient(x1, y1, z0, x, y, z);
        float p111 = dotGridGradient(x1, y1, z1, x, y, z);

        float r00 = interpolate(p000, p100, sx);
        float r01 = interpolate(p001, p101, sx);
        float r10 = interpolate(p010, p110, sx);
        float r11 = interpolate(p011, p111, sx);

        float s0 = interpolate(r00, r10, sy);
        float s1 = interpolate(r01, r11, sy);

        return interpolate(s0, s1, sz);
    }
}