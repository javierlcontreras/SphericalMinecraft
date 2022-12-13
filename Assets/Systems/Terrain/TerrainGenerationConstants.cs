using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationConstants {
    public static readonly string[] sideNameList = new string[] {
        "up",
        "down",
        "right",
        "left",
        "forward",
        "back"
    };
    public static readonly Vector3[] sideYaxisList = new Vector3[]{
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    public static readonly Vector3[] sideXaxisList = new Vector3[]{
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    public static readonly Vector3[] sideZaxisList = new Vector3[]{
        Vector3.left,
        Vector3.left,
        Vector3.back,
        Vector3.back,
        Vector3.down,
        Vector3.down
    };
    public static readonly int[] vertexOptions = new int[] {
        0,0,0, // left-bot-back
        1,0,0, // right-bot-back
        0,1,0, // left-top-back
        1,1,0, // right-top-back
        0,0,1, // left-bot-forw
        1,0,1, // right-bot-forw
        0,1,1, // left-top-forw
        1,1,1  // right-top-forw
    };
    public static readonly int[] sideOptions = new int[] {
        6,7,3,2, // top
        5,4,0,1, // bot
        7,5,1,3, // right
        4,6,2,0, // left
        7,6,4,5, // forward
        2,3,1,0 // back
    };
    
    public static float GetCoreRadius() {
        return 0.5f*0.5f*Mathf.Sqrt(2.0f);
    }

    private static int textureBlockSize = 128;
    private static int textureAtlasSize = 2048;
    public static float GetTextureBlockSize() {
        return 1f*textureBlockSize/textureAtlasSize;
    }

    public static Vector2[] ComputeUVsFromAtlasCoord(Vector2Int atlasCoord) {
        float s = GetTextureBlockSize();
        float x = atlasCoord.x*s;
        float y = 1 - atlasCoord.y*s;
        return new Vector2[] {
            new Vector2(x, y-s),
            new Vector2(x+s, y-s),
            new Vector2(x+s, y),
            new Vector2(x, y)
        };
    }
}
