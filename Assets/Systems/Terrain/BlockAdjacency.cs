using System.Linq;
using UnityEngine;

public class BlockAdjacency {
    private BlockCoordinateInformation[] blockCoords;
    private bool outOfBounds = false;

    public BlockAdjacency()
    {
        outOfBounds = true;
    }
    public BlockAdjacency(BlockCoordinateInformation block) {
        if (block == null)
        {
            Debug.LogWarning("Creating a blockadjacency to an inexistent block");
        }
        blockCoords = new BlockCoordinateInformation[1] {
            block
        };
    }

    public BlockAdjacency(BlockCoordinateInformation[] _blockCoords) {
        blockCoords = _blockCoords;
    }

    public BlockCoordinateInformation ClosestTo(Vector3 pointInGlobal)
    {
        float minDistance = float.PositiveInfinity;
        BlockCoordinateInformation closestOption = null;
        foreach (BlockCoordinateInformation blockCoord in blockCoords)
        {
            float distance = blockCoord.DistanceTo(pointInGlobal);
            if (minDistance > distance)
            {
                minDistance = distance;
                closestOption = blockCoord;
            }
        }
        return closestOption;
    }
    
    public bool IsAnyAir ()
    {
        if (outOfBounds) return true;
        foreach (BlockCoordinateInformation blockCoord in blockCoords)
        {
            Chunk chunk = blockCoord.GetPlanet().GetChunk(blockCoord.GetChunkCoords());
            if (chunk == null)
            {
                Debug.LogWarning("Asking for an adjancency of a chunk where data hasnt been loaded. This should not happen.");
                return true;
            }

            Block block = chunk.GetBlock(blockCoord.GetBlockCoords());
            if (block == null || block.GetBlockType().GetName() == "air") return true; 
        }
        return false;
    }
}