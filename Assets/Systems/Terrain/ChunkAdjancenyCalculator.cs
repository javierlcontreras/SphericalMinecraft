using System.Linq;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class ChunkAdjacencyCalculator {
    private PlanetTerrain planet;
    private int chunkSize;
    private int chunksPerSide;
    private int chunkHeight;

    private Vector3Int[] sideXaxisList;
    private Vector3Int[] sideYaxisList;
    private Vector3Int[] sideZaxisList;
    private string[] sideNameList;

    public ChunkAdjacencyCalculator(PlanetTerrain _planet) {
        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetChunkHeight();
        chunksPerSide = planet.GetChunksPerSide();
        sideXaxisList = TerrainGenerationConstants.sideXaxisList;
        sideYaxisList = TerrainGenerationConstants.sideYaxisList;
        sideZaxisList = TerrainGenerationConstants.sideZaxisList;
        sideNameList = TerrainGenerationConstants.sideNameList;
    }

    // pointing to is in chunk coordinate space
    // This function doesnt do what I expect. It seemed to suffice with a easier implementation for chunking
    // but I now need adjacency for block placing
    
    // pointing to can only be a cardinal direction
    public BlockAdjacency BlockNextToMe(Chunk chunk, Vector3Int blockCoord, Vector3Int pointingTo)
    {
        int sideCoord = chunk.GetSideCoord();
        int chunkX = chunk.GetXCoord();
        int chunkZ = chunk.GetZCoord();

        int nextX = blockCoord.x + pointingTo.x;
        int nextY = blockCoord.y + pointingTo.y;
        int nextZ = blockCoord.z + pointingTo.z;

        if (pointingTo.y != 0)
        {
            return BlockNextToMeVertically(chunk, blockCoord, pointingTo.y);
        }
        else
        {
            return BlockNextToMeHorizontally(chunk, blockCoord, pointingTo.x, pointingTo.z);
        }
    }

    public BlockAdjacency BlockNextToMeHorizontally(Chunk chunk, Vector3Int blockCoord, int dx, int dz)
    {
        int sideCoord = chunk.GetSideCoord();
        int chunkX = chunk.GetXCoord();
        int chunkZ = chunk.GetZCoord();
        
        int nextX = blockCoord.x + dx;
        int nextY = blockCoord.y;
        int nextZ = blockCoord.z + dz;

        int realChunkSize = (int) Mathf.Max(1, planet.NumBlocksAtHeightPerChunk(blockCoord.y));
        if (inRange(nextX, 0, realChunkSize) && inRange(nextZ, 0, realChunkSize))
        {
            return new BlockAdjacency(chunk.GetBlock(nextX, nextY, nextZ));
        }
        else
        {
            Vector3Int chunkNext = ChunkNextToMe(sideCoord, chunkX, chunkZ, dx, dz);
            Vector3Int blockNext = BlockClosestToMeInChunk(blockCoord, new Vector3Int(sideCoord, chunkX, chunkZ), chunkNext);
            int sideNextCoord = chunkNext.x;
            int chunkNextX = chunkNext.y;
            int chunkNextZ = chunkNext.z;
            nextX = blockNext.x;
            nextZ = blockNext.z;

            if (planet.chunks[sideNextCoord, chunkNextX, chunkNextZ] == null)
            {
                Debug.LogWarning("A chunk mesh is being created before its chunk neighbors data is computed!");
            }
                
            return new BlockAdjacency(planet.chunks[sideNextCoord, chunkNextX, chunkNextZ].GetBlock(nextX, nextY, nextZ));
        }
    }

    public Vector3Int BlockClosestToMeInChunk(Vector3Int blockCoord, Vector3Int chunkCoord, Vector3Int chunkNextCoord)
    {
        int side = chunkCoord.x;
        int chunkX = chunkCoord.y;
        int chunkZ = chunkCoord.z;
        int x = blockCoord.x;
        int y = blockCoord.y;
        int z = blockCoord.z;

        int nextSide = chunkNextCoord.x;
        int nextChunkX = chunkNextCoord.y;
        int nextChunkZ = chunkNextCoord.z;

        int realChunkSize = planet.NumBlocksAtHeightPerChunk(y);
        int realChunkSizeMinus1 = realChunkSize - 1;
        int[] options = new int[16] {
            x, z,
            realChunkSizeMinus1 - x, z,
            x, realChunkSizeMinus1 - z,
            realChunkSizeMinus1 - x, realChunkSizeMinus1 - z,
            z, x,
            z, realChunkSizeMinus1 - x,
            realChunkSizeMinus1 - z, realChunkSizeMinus1 - x,
            realChunkSizeMinus1 - z, x
        };
        float minDistance = float.PositiveInfinity;
        Vector3Int minOption = new Vector3Int(0, y, 0);
        for (int option = 0; option < 8; option += 1)
        {
            int nextX = options[2*option];
            int nextZ = options[2*option+1];

            float distance = DistanceBetweenBlocks(realChunkSize, side, chunkX, chunkZ, x, z, nextSide, nextChunkX,
                nextChunkZ, nextX, nextZ);
            if (distance < minDistance)
            {
                minDistance = distance;
                minOption.x = nextX;
                minOption.z = nextZ;
            }
        }

        return minOption;
    }

    public float DistanceBetweenBlocks(int realChunkSize, int side, int chunkX, int chunkZ, int x, int z, int nextSide,
        int nextChunkX, int nextChunkZ, int nextX, int nextZ)
    {
        Vector3Int point1 = BlockVectorOnCube(realChunkSize, side, chunkX, chunkZ, x, z);
        Vector3Int point2 = BlockVectorOnCube(realChunkSize, nextSide, nextChunkX, nextChunkZ, nextX, nextZ);
        return (point1 - point2).sqrMagnitude;
    }

    public Vector3Int BlockVectorOnCube(int realChunkSize, int side, int chunkX, int chunkZ, int x, int z)
    {
        int radius = chunksPerSide * realChunkSize;
        int diffX = 2*(chunkX * realChunkSize + x) + 1 - radius;
        int diffZ = 2*(chunkZ * realChunkSize + z) + 1 - radius;
        return sideYaxisList[side] * radius + sideXaxisList[side] * diffX + sideZaxisList[side] * diffZ;
    }
    
    public BlockAdjacency BlockNextToMeVertically(Chunk chunk, Vector3Int blockCoord, int dy)
    {
        int sideCoord = chunk.GetSideCoord();
        int chunkX = chunk.GetXCoord();
        int chunkZ = chunk.GetZCoord();
        
        int nextX = blockCoord.x;
        int nextY = blockCoord.y + dy;
        int nextZ = blockCoord.z;
        
        if (!inRange(nextY, 0, chunkHeight))
        {
            return new BlockAdjacency();
        }

        int realChunkSize = planet.NumBlocksAtHeightPerChunk(blockCoord.y);
        int nextRealChunkSize = planet.NumBlocksAtHeightPerChunk(nextY);
        if (nextRealChunkSize < realChunkSize)
        {
            if (!inRange(nextX / 2, 0, nextRealChunkSize) || !inRange(nextZ / 2, 0, nextRealChunkSize))
            {
                return new BlockAdjacency();
            }

            Block block = chunk.GetBlock(nextX / 2, nextY, nextZ / 2);
            return new BlockAdjacency(block);
        }
        else if (nextRealChunkSize > realChunkSize)
        {
            if (!inRange(2 * nextX + 1, 0, nextRealChunkSize) || !inRange(2 * nextZ + 1, 0, nextRealChunkSize))
            {
                return null;
            }

            Block[] blocks = new Block[4]
            {
                chunk.GetBlock(2 * nextX, nextY, 2 * nextZ),
                chunk.GetBlock(2 * nextX, nextY, 2 * nextZ + 1),
                chunk.GetBlock(2 * nextX + 1, nextY, 2 * nextZ),
                chunk.GetBlock(2 * nextX + 1, nextY, 2 * nextZ + 1),
            };
            return new BlockAdjacency(blocks);
        }
        else
        {
            if (!inRange(nextX, 0, nextRealChunkSize) || !inRange(nextZ, 0, nextRealChunkSize))
            {
                return null;
            }

            Block block = chunk.GetBlock(nextX, nextY, nextZ);
            return new BlockAdjacency(block);
        }
    
    }

    public Vector3Int ChunkNextToMe(Vector3Int chunkCoord, int dx, int dz)
    {
        return ChunkNextToMe(chunkCoord.x, chunkCoord.y, chunkCoord.z, dx, dz);
    }
    public Vector3Int ChunkNextToMe(int sideCoord, int chunkX, int chunkZ, int dx, int dz)
    {
        int nextChunkX = chunkX + dx;
        int nextChunkZ = chunkZ + dz;
        if (inRange(nextChunkX, 0, planet.GetChunksPerSide()) && inRange(nextChunkZ, 0, planet.GetChunksPerSide()))
        {
            return new Vector3Int(sideCoord, nextChunkX, nextChunkZ);
        }
        else
        {
            int sideCoordNextToMe = SideCoordNextToMe(sideCoord, dx, dz);
            return ChunkNextToMeInSide(sideCoord, chunkX, chunkZ, sideCoordNextToMe);
        }
    }

    public Vector3Int ChunkNextToMeInSide(int sideCoord, int chunkX, int chunkZ, int sideCoordNextToMe)
    {
        if (sideCoord == sideCoordNextToMe)
        {
            Debug.LogWarning("Looking for closest chunk in same side.");
        }
        int chunksPerSideMinus1 = planet.GetChunksPerSide() - 1;
        int[] options = new int[16] {
            chunkX, chunkZ,
            chunksPerSideMinus1 - chunkX, chunkZ,
            chunkX, chunksPerSideMinus1 - chunkZ,
            chunksPerSideMinus1 - chunkX, chunksPerSideMinus1 - chunkZ,
            chunkZ, chunkX,
            chunkZ, chunksPerSideMinus1 - chunkX,
            chunksPerSideMinus1 - chunkZ, chunksPerSideMinus1 - chunkX,
            chunksPerSideMinus1 - chunkZ, chunkX
        };
        float mindist = float.PositiveInfinity; 
        Vector3Int minOption = new Vector3Int(sideCoordNextToMe, 0, 0);
        for (int opt=0; opt<16; opt+=2) {
            int nextChunkX = options[opt]; 
            int nextChunkZ = options[opt+1]; 
            float dist = DistanceChunkToChunk(sideCoord, chunkX, chunkZ, sideCoordNextToMe, nextChunkX, nextChunkZ);
            if (mindist > dist) {
                mindist = dist;
                minOption.y = nextChunkX; 
                minOption.z = nextChunkZ; 
            }
        }
        Debug.Log(sideCoord + " " + chunkX + " " +  chunkZ + " " +  minOption);
        return minOption;
    }

    public float DistanceChunkToChunk(int sideCoord, int chunkX, int chunkZ, int nextSideCoord, int nextChunkX,
        int nextChunkZ)
    {
        int chunksPerSide = planet.GetChunksPerSide();
        Vector3Int positionChunk1 = ChunkBaseVectorCube(sideCoord, chunkX, chunkZ);
        Vector3Int positionChunk2 = ChunkBaseVectorCube(nextSideCoord, nextChunkX, nextChunkZ);
        return (positionChunk1 - positionChunk2).sqrMagnitude;
    }

    public Vector3Int ChunkBaseVectorCube(int sideCoord, int chunkX, int chunkZ)
    {
        return chunksPerSide * sideYaxisList[sideCoord] + 
            (2*chunkX - chunksPerSide + 1) * sideXaxisList[sideCoord] +
            (2*chunkZ - chunksPerSide + 1) * sideZaxisList[sideCoord];
    }
    public int SideCoordNextToMe(int sideCoord, int dx, int dz)
    {
        int side = 0;
        if (dx != 0)
        {
            for (int s = 0; s < 6; s++)
            {
                if (sideYaxisList[s] == dx * sideXaxisList[sideCoord]) side = s;
            } 
        }
        else
        {
            for (int s = 0; s < 6; s++)
            {
                if (sideYaxisList[s] == dz * sideZaxisList[sideCoord]) side = s;
            } 
        }   
        return side;
    }
    
    public bool inRange(int a, int l, int r) {
        return l <= a && a < r;
    }

}
