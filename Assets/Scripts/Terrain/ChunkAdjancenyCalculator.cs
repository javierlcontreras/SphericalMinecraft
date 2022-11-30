using UnityEngine;

public class ChunkAdjacencyCalculator {
    private Planet planet;
    private int chunkSize;
    private int chunksPerSide;
    private int chunkHeight;

    private Vector3[] sideXaxisList;
    private Vector3[] sideYaxisList;
    private Vector3[] sideZaxisList;
    private string[] sideNameList;

    public ChunkAdjacencyCalculator(Planet _planet, Vector3[] _sideXaxisList, Vector3[] _sideYaxisList, Vector3[] _sideZaxisList, string[] _sideNameList) {
        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetChunkHeight();
        chunksPerSide = planet.GetChunksPerSide();
        sideXaxisList = _sideXaxisList;
        sideYaxisList = _sideYaxisList;
        sideZaxisList = _sideZaxisList;
        sideNameList = _sideNameList;
    }

    public Block BlockNextToMe(Chunk chunk, int x, int y, int h, Vector3 pointingTo, Vector3 pointingToGlobal) { // pointing to is in chunk coordinate space
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkY = chunk.yCoord;

        //Debug.Log(sideNameList[sideCoord] + " " + pointingTo + " " + pointingToGlobal);

        int nextX = x + (int)pointingTo.x; 
        int nextY = y + (int)pointingTo.z; 
        int nextH = h + (int)pointingTo.y; 
        
        if (inRange(nextX, 0, chunkSize) && inRange(nextY, 0, chunkSize) && inRange(nextH, 0, chunkHeight)) {
            return planet.chunks[sideCoord, chunkX, chunkY].blocks[nextX,nextY,nextH];
        }
        if (!inRange(nextH, 0, chunkHeight)) {
            return new Block(-1,-1,-1,BlockTypeEnum.GetBlockTypeByName("air"));
        }
        Chunk nextChunk = ChunkNextToMe(chunk, pointingTo, pointingToGlobal);
        if (nextChunk.sideCoord == sideCoord) {
            int chunkNextI = (nextX + chunkSize)%chunkSize;
            int chunkNextJ = (nextY + chunkSize)%chunkSize;
            return nextChunk.blocks[chunkNextI,chunkNextJ,nextH]; 
        } 

        Block nextBlock = blockClosestToInChunk(chunk, x, y, h, nextChunk);
        //chunk.blocks[x,y,h].DrawSphere();
        //nextBlock.DrawDebugSphere();
        return nextBlock; 
    }
    private Block blockClosestToInChunk(Chunk chunk, int x, int y, int h, Chunk nextChunk) {
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkY = chunk.yCoord;

        int nBlocks1 = chunkSize - 1;
        int[] options = new int[16] {
            x, y,
            nBlocks1 - x, y,
            x, nBlocks1 - y,
            nBlocks1 - x, nBlocks1 - y,
            y, x,
            y, nBlocks1 - x,
            nBlocks1 - y, nBlocks1 - x,
            nBlocks1 - y, x
        };
        float mindist = 10000; 
        Block minOpt = null;
        for (int opt=0; opt<16; opt+=2) {
            int nextX = options[opt]; 
            int nextY = options[opt+1]; 
            float dist = distanceBlockToBlock(chunk, x, y, nextChunk, nextX, nextY);
            if (mindist > dist) {
                mindist = dist;
                minOpt = nextChunk.blocks[nextX, nextY, h];
            }
        }
        //Debug.Log(sideNameList[sideCoord] + " " + x + " " + y);
        //Debug.Log(sideNameList[nextChunk.sideCoord] + " " + minOpt.xInChunk + " " + minOpt.yInChunk);
        //Debug.Log("----");
        return minOpt;
    }
    private float distanceBlockToBlock(Chunk chunk, int x, int y, Chunk nextChunk, int nextX, int nextY) {
        int sideCoord1 = chunk.sideCoord;
        Vector3 base1 = TerrainManager.instance.BaseVector(sideCoord1, chunk.xCoord, chunk.yCoord, x, y);

        int sideCoord2 = nextChunk.sideCoord;
        Vector3 base2 = TerrainManager.instance.BaseVector(sideCoord2, nextChunk.xCoord, nextChunk.yCoord, nextX, nextY);

        return (base1 - base2).magnitude;
    }
/*
    public Quaternion RotationFromSideToSide(int side, int sideNext) {
        Vector3 sideNormal = sideYaxisList[sideNext];
        Vector3 sideXaxis = sideZaxisList[sideNext];

        return Quaternion.LookRotation(sideZaxis, sideNormal);
    }
*/
    public Chunk ChunkNextToMe(Chunk chunk, Vector3 pointingTo, Vector3 pointingToGlobal) {
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkY = chunk.yCoord;

        int chunkNextX = chunkX + (int)pointingTo.x;
        int chunkNextY = chunkY + (int)pointingTo.z;
        
        if (inRange(chunkNextX, 0, chunksPerSide) && inRange(chunkNextY, 0, chunksPerSide)) {
            return planet.chunks[sideCoord, chunkNextX, chunkNextY]; 
        }
        int sideNext = SideNextToMe(sideCoord, pointingToGlobal);
        
        Chunk nextChunk = closestChunkToInSide(chunk, sideNext);
        // TODO: debug with > 1 chunks!
        
        //Debug.Log(sideNameList[sideCoord] + "," + chunkX + "," + chunkY);
        //Debug.Log(sideNameList[nextChunk.sideCoord] + "," + nextChunk.xCoord + "," + nextChunk.yCoord);
        //Debug.Log("----");
        return nextChunk;
    }
    
    private Chunk closestChunkToInSide(Chunk chunk, int sideNext) {
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkY = chunk.yCoord;

        int nChunk1 = chunksPerSide - 1;
        int[] options = new int[16] {
            chunkX, chunkY,
            nChunk1 - chunkX, chunkY,
            chunkX, nChunk1 - chunkY,
            nChunk1 - chunkX, nChunk1 - chunkY,
            chunkY, chunkX,
            chunkY, nChunk1 - chunkX,
            nChunk1 - chunkY, nChunk1 - chunkX,
            nChunk1 - chunkY, chunkX
        };
        float mindist = 10000; 
        Chunk minOpt = null;
        for (int opt=0; opt<16; opt+=2) {
            int nextChunkX = options[opt]; 
            int nextChunkY = options[opt+1]; 
            Chunk chunkNext = planet.chunks[sideNext, nextChunkX, nextChunkY];
            float dist = chunk.DistanceToChunk(chunkNext);
            if (mindist > dist) {
                mindist = dist;
                minOpt = chunkNext;
            }
        }
        return minOpt;
    }

    public int SideNextToMe(int sideCoord, Vector3 pointingToGlobal) {
        int nextSide = -1;
        for (int side=0; side<6; side++) {
            if (sideYaxisList[side] == pointingToGlobal) nextSide = side;
        }

        //Debug.Log(sideNameList[sideCoord] + " " + pointingToGlobal + " " + sideNameList[nextSide]);
        return nextSide;
    }

    public bool inRange(int a, int l, int r) {
        return l <= a && a < r;
    }

}