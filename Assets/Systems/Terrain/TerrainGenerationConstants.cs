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
    public static readonly Vector3Int[] sideYaxisList = new Vector3Int[]{
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.right,
        Vector3Int.left,
        Vector3Int.forward,
        Vector3Int.back
    };
    public static readonly Vector3Int[] sideXaxisList = new Vector3Int[]{
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.right,
        Vector3Int.left
    };
    public static readonly Vector3Int[] sideZaxisList = new Vector3Int[]{
        Vector3Int.left,
        Vector3Int.left,
        Vector3Int.back,
        Vector3Int.back,
        Vector3Int.down,
        Vector3Int.down
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

    private static float blockSize = 1; 
    private static float blockHeight = 1; 
    public static float GetBlockSize() {
        return blockSize;
    }
    public static float GetBlockHeight() {
        return blockHeight;
    }

    private static int textureBlockSize = 1;
    private static int textureAtlasSize = 16;
    public static float GetTextureBlockSize() {
        return 1f*textureBlockSize/textureAtlasSize;
    }
    public static float GetTextureBlockMargin() {
        return 1f*(textureBlockSize-1f/32f)/textureAtlasSize;
    }

    public static Vector2[] ComputeUVsFromAtlasCoord(Vector2Int atlasCoord) {
        float s = GetTextureBlockSize();
        float m = GetTextureBlockMargin();
        float f = s - 2*m;
        float x = atlasCoord.x*s + m;
        float y = 1 - atlasCoord.y*s - m;
        return new Vector2[] {
            new Vector2(x, y-f),
            new Vector2(x+f, y-f),
            new Vector2(x+f, y),
            new Vector2(x, y)
        };
    }
}
