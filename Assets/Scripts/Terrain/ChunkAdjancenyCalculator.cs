using UnityEngine;

public class ChunkAdjacencyCalculator {
    private Planet planet;
    private int chunkSize;
    private int chunksPerSide;
    private int chunkHeight;

    private Vector3[] sideNormalList;
    private Vector3[] sideTangentList;

    public ChunkAdjacencyCalculator(Planet _planet, Vector3[] _sideNormalList, Vector3[] _sideTangentList) {
        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetChunkHeight();
        chunksPerSide = planet.GetChunksPerSide();
        sideNormalList = _sideNormalList;
        sideTangentList = _sideTangentList;
    }

    public Block BlockNextToMe(int sideCoord, int chunkX, int chunkY, int x, int y, int h, Vector3 pointingTo, Vector3 pointingToGlobal) { // pointing to is in chunk coordinate space
        int nextX = x + (int)pointingTo.x; 
        int nextY = y + (int)pointingTo.z; 
        int nextH = h + (int)pointingTo.y; 
        
        if (inRange(nextX, 0, chunkSize) && inRange(nextY, 0, chunkSize) && inRange(nextH, 0, chunkHeight)) {
            return planet.chunks[sideCoord, chunkX, chunkY].blocks[nextX,nextY,nextH];
        }
        if (!inRange(nextH, 0, chunkHeight)) {
            return new Block(-1,-1,-1,BlockTypeEnum.GetBlockTypeByName("air"));
        }
        Chunk nextChunk = ChunkNextToMe(sideCoord, chunkX, chunkY, pointingTo);
        if (nextChunk != null) {
            int chunkNextI = (nextX + chunkSize)%chunkSize;
            int chunkNextJ = (nextY + chunkSize)%chunkSize;
            return nextChunk.blocks[chunkNextI,chunkNextJ,nextH]; 
        } 

        //int sideNext = SideNextToMe(sideCoord, pointingToGlobal);
        //Quaternion sideToNextSide = RotationFromSideToSide(side, sideNext);

        // TODO: spin the cube!

        return new Block(-1,-1,-1,BlockTypeEnum.GetBlockTypeByName("air"));
        
    }

    public Quaternion RotationFromSideToSide(int side, int sideNext) {
        Vector3 sideNormal = sideNormalList[sideNext];
        Vector3 sideXaxis = sideTangentList[sideNext];

        return Quaternion.LookRotation(sideXaxis, sideNormal);
    }

    public Chunk ChunkNextToMe(int sideCoord, int chunkX, int chunkY, Vector3 pointingTo) {
        int chunkNextX = chunkX + (int)pointingTo.x;
        int chunkNextY = chunkY + (int)pointingTo.z;
        
        if (inRange(chunkNextX, 0, chunksPerSide) && inRange(chunkNextY, 0, chunksPerSide)) {
            return planet.chunks[sideCoord, chunkNextX, chunkNextY]; 
        }
        
        // TODO: spin the cube!

        return null;
    }

    public int SideNextToMe(int sideCoord, Vector3 pointingToGlobal) {
        for (int side=0; side<6; side++) {
            if (sideNormalList[side] == pointingToGlobal) return side;
        }
        return -1;
    }

    public bool inRange(int a, int l, int r) {
        return l <= a && a < r;
    }

}