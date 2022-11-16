using UnityEngine;

public class ChunkAdjacencyCalculator {
    private Planet planet;
    private int chunkSize;
    private int chunksPerSide;
    private int chunkHeight;

    public ChunkAdjacencyCalculator(Planet _planet, Vector3[] sideNormals, Vector3[] sideTangents) {
        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetChunkHeight();
        chunksPerSide = planet.GetChunksPerSide();
    }

    public Block BlockNextToMe(int sideCoord, int chunkX, int chunkY, int x, int y, int h, Vector3 pointingTo, int sideNext) { // pointing to is in chunk coordinate space
        int nextX = x + (int)pointingTo.x; 
        int nextY = y + (int)pointingTo.z; 
        int nextH = h + (int)pointingTo.y; 
        
        if (nextX >= chunkSize || nextX < 0 || nextY >= chunkSize || nextY < 0 || nextH >= chunkHeight || nextH < 0) 
            return new Block(-1,-1,-1,BlockTypeEnum.GetBlockTypeByName("air"));
        
        return planet.chunks[sideCoord, chunkX, chunkY].blocks[nextX,nextY,nextH];
        
        /*
        if (nextH < 0) return "bedrock";
        if (nextH >= chunkHeight) return "air";

        

        string blockTypeNext = "air";
        if (nextI >= chunkSize || nextI < 0 || nextJ >= chunkSize || nextJ < 0 || nextH >= chunkHeight || nextH < 0) {
            int nextChunkX = xCoord + pointingTo.x;
            int nextChunkY = yCoord + pointingTo.z;

            if (nextChunkX < 0 || nextChunkX >= chunksPerSide || nextChunkY < 0 || nextChunkY >= chunksPerSide) {


            }

        }
        else {
        }
        return 
        */
    }

    public Chunk ChunkNextToMe(int sideCoord, int chunkX, int chunkY, Vector3 pointingTo) {
        return null;
    }

    public int SideNextToMe(int sideCoord, Vector3 pointingTo) {
        return 0;
    }

}