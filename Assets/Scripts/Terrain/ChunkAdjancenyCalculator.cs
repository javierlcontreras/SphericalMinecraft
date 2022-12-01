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

    public Block BlockNextToMe(Chunk chunk, int x, int z, int h, Vector3 pointingTo, Vector3 pointingToGlobal) { // pointing to is in chunk coordinate space
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkZ = chunk.zCoord;

        //Debug.Log(sideNameList[sideCoord] + " " + pointingTo + " " + pointingToGlobal);

        int nextX = x + (int)pointingTo.x; 
        int nextZ = z + (int)pointingTo.z; 
        int nextH = h + (int)pointingTo.y; 
        
        if (inRange(nextX, 0, chunkSize) && inRange(nextZ, 0, chunkSize) && inRange(nextH, 0, chunkHeight)) {
            return planet.chunks[sideCoord, chunkX, chunkZ].blocks[nextX,nextH,nextZ];
        }
        if (!inRange(nextH, 0, chunkHeight)) {
            return new Block(new Vector3(-1,-1,-1),BlockTypeEnum.GetBlockTypeByName("air"), null);
        }
        Chunk nextChunk = ChunkNextToMe(chunk, pointingTo, pointingToGlobal);
        if (nextChunk.sideCoord == sideCoord) {
            int chunkNextX = (nextX + chunkSize)%chunkSize;
            int chunkNextZ = (nextZ + chunkSize)%chunkSize;
            return nextChunk.blocks[chunkNextX,nextH,chunkNextZ]; 
        } 

        Block nextBlock = blockClosestToInChunk(chunk, x, z, h, nextChunk);
        //chunk.blocks[x,y,h].DrawSphere();
        //nextBlock.DrawDebugSphere();
        return nextBlock; 
    }
    private Block blockClosestToInChunk(Chunk chunk, int x, int z, int h, Chunk nextChunk) {
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkZ = chunk.zCoord;

        int nBlocks1 = chunkSize - 1;
        int[] options = new int[16] {
            x, z,
            nBlocks1 - x, z,
            x, nBlocks1 - z,
            nBlocks1 - x, nBlocks1 - z,
            z, x,
            z, nBlocks1 - x,
            nBlocks1 - z, nBlocks1 - x,
            nBlocks1 - z, x
        };
        float mindist = 10000; 
        Block minOpt = null;
        for (int opt=0; opt<16; opt+=2) {
            int nextX = options[opt]; 
            int nextZ = options[opt+1]; 
            float dist = distanceBlockToBlock(chunk, x, z, nextChunk, nextX, nextZ);
            if (mindist > dist) {
                mindist = dist;
                minOpt = nextChunk.blocks[nextX, h, nextZ];
            }
        }
        //Debug.Log(sideNameList[sideCoord] + " " + x + " " + y);
        //Debug.Log(sideNameList[nextChunk.sideCoord] + " " + minOpt.xInChunk + " " + minOpt.yInChunk);
        //Debug.Log("----");
        return minOpt;
    }
    private float distanceBlockToBlock(Chunk chunk, int blockX, int blockZ, Chunk nextChunk, int nextBlockX, int nextBlockZ) {
        int sideCoord1 = chunk.sideCoord;
        Vector3 base1 = TerrainManager.instance.BaseVector(sideCoord1, chunk.xCoord, chunk.zCoord, blockX, blockZ);

        int sideCoord2 = nextChunk.sideCoord;
        Vector3 base2 = TerrainManager.instance.BaseVector(sideCoord2, nextChunk.xCoord, nextChunk.zCoord, nextBlockX, nextBlockZ);

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
        int chunkZ = chunk.zCoord;

        int chunkNextX = chunkX + (int)pointingTo.x;
        int chunkNextZ = chunkZ + (int)pointingTo.z;
        
        if (inRange(chunkNextX, 0, chunksPerSide) && inRange(chunkNextZ, 0, chunksPerSide)) {
            return planet.chunks[sideCoord, chunkNextX, chunkNextZ]; 
        }
        int sideNext = SideNextToMe(sideCoord, pointingToGlobal);
        
        Chunk nextChunk = closestChunkToInSide(chunk, sideNext);
        
        return nextChunk;
    }
    
    private Chunk closestChunkToInSide(Chunk chunk, int sideNext) {
        int sideCoord = chunk.sideCoord; 
        int chunkX = chunk.xCoord;
        int chunkZ = chunk.zCoord;

        int nChunk1 = chunksPerSide - 1;
        int[] options = new int[16] {
            chunkX, chunkZ,
            nChunk1 - chunkX, chunkZ,
            chunkX, nChunk1 - chunkZ,
            nChunk1 - chunkX, nChunk1 - chunkZ,
            chunkZ, chunkX,
            chunkZ, nChunk1 - chunkX,
            nChunk1 - chunkZ, nChunk1 - chunkX,
            nChunk1 - chunkZ, chunkX
        };
        float mindist = 10000; 
        Chunk minOpt = null;
        for (int opt=0; opt<16; opt+=2) {
            int nextChunkX = options[opt]; 
            int nextChunkZ = options[opt+1]; 
            Chunk chunkNext = planet.chunks[sideNext, nextChunkX, nextChunkZ];
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