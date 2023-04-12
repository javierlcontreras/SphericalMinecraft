using System.Linq;
using Unity.Collections;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class SphericalBlockAdjacencyCalculator {
    private PlanetTerrain planet;

    public SphericalBlockAdjacencyCalculator(PlanetTerrain _planet) {
        planet = _planet;
    }

    // pointing to is in chunk coordinate space
    // This function doesnt do what I expect. It seemed to suffice with a easier implementation for chunking
    // but I now need adjacency for block placing
    // pointing to can only be a cardinal direction
    public BlockAdjacency BlockNextToMe(Vector3Int blockCoord, Vector3Int chunkCoord, Vector3Int pointingTo)
    {
        return BlockNextToMe(new BlockCoordinateInformation(blockCoord, chunkCoord, planet), pointingTo);
    }
    public BlockAdjacency BlockNextToMe(BlockCoordinateInformation blockCoordInfo, Vector3Int pointingTo)
    {
        BlockAdjacency adj = BlockNextToMeHorizontally(blockCoordInfo, pointingTo.x, pointingTo.z);
        BlockCoordinateInformation adjBlock = adj.GetBlockIfUnique();
        return BlockNextToMeVertically(adjBlock, pointingTo.y);
    }

    public BlockAdjacency BlockNextToMeHorizontally(BlockCoordinateInformation blockCoordInfo, int dx, int dz)
    {
        if (dx == 0 && dz == 0)
        {
            return new BlockAdjacency(blockCoordInfo);
        }
        if (dx != 0 && dz != 0)
        {
            // TODO: this is terribly bugged because of holonomy. But its only used for ambient occlusion right now
            BlockCoordinateInformation oneSide = BlockNextToMeHorizontally(blockCoordInfo, dx, 0).GetBlockIfUnique();
            return BlockNextToMeHorizontally(oneSide, 0, dz);
        }
        
        Vector3Int blockCoord = blockCoordInfo.GetBlockCoords();
        Vector3Int chunkCoord = blockCoordInfo.GetChunkCoords();
        
        int sideCoord = chunkCoord.x;
        int chunkX = chunkCoord.y;
        int chunkZ = chunkCoord.z;
        
        int nextX = blockCoord.x + dx;
        int nextY = blockCoord.y;
        int nextZ = blockCoord.z + dz;

        int realChunkSize = (int) Mathf.Max(1, planet.NumBlocksAtHeightPerChunk(blockCoord.y));
        if (inRange(nextX, 0, realChunkSize) && inRange(nextZ, 0, realChunkSize))
        {
            return new BlockAdjacency(new BlockCoordinateInformation(new Vector3Int(nextX, nextY, nextZ), chunkCoord, planet));
        }
        else
        {
            Vector3Int chunkNext = ChunkNextToMe(sideCoord, chunkX, chunkZ, dx, dz);
            Vector3Int blockNext = BlockClosestToMeInChunk(blockCoord, chunkCoord, chunkNext);
            int sideNextCoord = chunkNext.x;
            int chunkNextX = chunkNext.y;
            int chunkNextZ = chunkNext.z;
            nextX = blockNext.x;
            nextZ = blockNext.z;

            if (planet.GetChunk(sideNextCoord, chunkNextX, chunkNextZ) == null)
            {
                Debug.LogWarning("A chunk mesh is being created before its chunk neighbors data is computed!");
            }
                
            return new BlockAdjacency(new BlockCoordinateInformation(
                blockNext,
                chunkNext,
                planet
                ));
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
        int chunksPerSide = planet.GetChunksPerSide();
        int radius = chunksPerSide * realChunkSize;
        int diffX = 2*(chunkX * realChunkSize + x) + 1 - radius;
        int diffZ = 2*(chunkZ * realChunkSize + z) + 1 - radius;
        return TerrainGenerationConstants.sideYaxisList[side] * radius 
               + TerrainGenerationConstants.sideXaxisList[side] * diffX 
               + TerrainGenerationConstants.sideZaxisList[side] * diffZ;
    }
    
    public BlockAdjacency BlockNextToMeVertically(BlockCoordinateInformation blockCoordInfo, int dy)
    {
        if (dy == 0)
        {
            return new BlockAdjacency(blockCoordInfo);
        }
        Vector3Int blockCoord = blockCoordInfo.GetBlockCoords();
        Vector3Int chunkCoord = blockCoordInfo.GetChunkCoords();

        int nextX = blockCoord.x;
        int nextY = blockCoord.y + dy;
        int nextZ = blockCoord.z;

        int chunkHeight = planet.GetChunkHeight();
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

            return new BlockAdjacency(new BlockCoordinateInformation(
                new Vector3Int(nextX/2, nextY, nextZ/2),
                chunkCoord,
                planet
                ));
        }
        else if (nextRealChunkSize > realChunkSize)
        {
            if (!inRange(2 * nextX + 1, 0, nextRealChunkSize) || !inRange(2 * nextZ + 1, 0, nextRealChunkSize))
            {
                return new BlockAdjacency();
            }

            BlockCoordinateInformation[] blockCoords = new BlockCoordinateInformation[4]
            {
                new BlockCoordinateInformation(new Vector3Int(2 * nextX, nextY, 2 * nextZ), chunkCoord, planet),
                new BlockCoordinateInformation(new Vector3Int(2 * nextX + 1, nextY, 2 * nextZ), chunkCoord, planet),
                new BlockCoordinateInformation(new Vector3Int(2 * nextX, nextY, 2 * nextZ + 1), chunkCoord, planet),
                new BlockCoordinateInformation(new Vector3Int(2 * nextX + 1, nextY, 2 * nextZ + 1), chunkCoord, planet)
            };
            return new BlockAdjacency(blockCoords);
        }
        else
        {
            if (!inRange(nextX, 0, nextRealChunkSize) || !inRange(nextZ, 0, nextRealChunkSize))
            {
                return null;
            }

            return new BlockAdjacency(new BlockCoordinateInformation(
                new Vector3Int(nextX, nextY, nextZ),
                chunkCoord,
                planet
                ));
        }
    
    }

    public Vector3Int ChunkNextToMe(Vector3Int chunkCoord, int dx, int dz)
    {
        return ChunkNextToMe(chunkCoord.x, chunkCoord.y, chunkCoord.z, dx, dz);
    }
    public Vector3Int ChunkNextToMe(int sideCoord, int chunkX, int chunkZ, int dx, int dz)
    {
        if (dx == 0 && dz == 0)
        {
            return new Vector3Int(sideCoord, chunkX, chunkZ);
        }

        if (dx != 0 && dz != 0)
        {   
            // TODO: this is terribly bugged because of holonomy. But its only used for ambient occlusion right now
            Vector3Int sideOne = ChunkNextToMe(sideCoord, chunkX, chunkZ, dx, 0);
            return ChunkNextToMe(sideOne.x, sideOne.y, sideOne.z, 0, dz);
        }
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
        int chunksPerSide = planet.GetChunksPerSide();
        return chunksPerSide * TerrainGenerationConstants.sideYaxisList[sideCoord] + 
            (2*chunkX - chunksPerSide + 1) * TerrainGenerationConstants.sideXaxisList[sideCoord] +
            (2*chunkZ - chunksPerSide + 1) * TerrainGenerationConstants.sideZaxisList[sideCoord];
    }
    public int SideCoordNextToMe(int sideCoord, int dx, int dz)
    {
        int side = 0;
        if (dx != 0)
        {
            for (int s = 0; s < 6; s++)
            {
                if (TerrainGenerationConstants.sideYaxisList[s] == 
                    dx * TerrainGenerationConstants.sideXaxisList[sideCoord]) side = s;
            } 
        }
        else
        {
            for (int s = 0; s < 6; s++)
            {
                if (TerrainGenerationConstants.sideYaxisList[s] == 
                    dz * TerrainGenerationConstants.sideZaxisList[sideCoord]) side = s;
            } 
        }   
        return side;
    }
    
    public bool inRange(int a, int l, int r) {
        return l <= a && a < r;
    }

    public BlockAdjacency[] BlocksTouchingAVertex(BlockCoordinateInformation block, Vector3Int corner)
    {
        int x = corner.x;
        int y = corner.y;
        int z = corner.z;
        return new BlockAdjacency[8]
        {
            new BlockAdjacency(block),
            BlockNextToMe(block, new Vector3Int(x, 0, 0)),
            BlockNextToMe(block, new Vector3Int(0, y, 0)),
            BlockNextToMe(block, new Vector3Int(0, 0, z)),
            BlockNextToMe(block, new Vector3Int(x, y, 0)),
            BlockNextToMe(block, new Vector3Int(x, 0, z)),
            BlockNextToMe(block, new Vector3Int(0, y, z)),
            BlockNextToMe(block, new Vector3Int(x, y, z)),
        };
    }
}
