using UnityEngine;

public class BlockSide {
    Vector3[] vertices;
    Vector2[] uvs;
    

    public BlockSide(Vector3[] _vertices, Vector2 atlasCoord) {
        vertices = _vertices;
        uvs = computeUVsFromAtlas(atlasCoord);
    }

    private Vector2[] computeUVsFromAtlas(Vector2 atlasCoord) {
        float s = 1f*TerrainManager.instance.textureBlockSize/TerrainManager.instance.textureAtlasSize;
        float x = (int)atlasCoord.x*s;
        float y = 1-(int)atlasCoord.y*s;
        return new Vector2[] {
            new Vector2(x, y),
            new Vector2(x+s, y),
            new Vector2(x+s, y+s),
            new Vector2(x, y+s)
        };
    }

    public BlockSideTriangle[] GetTriangles() {
        BlockSideTriangle[] triang = new BlockSideTriangle[2];
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2], uvs[0], uvs[1], uvs[2]);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3], uvs[0], uvs[2], uvs[3]);
        return triang;
    }
}