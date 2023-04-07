using UnityEngine;

public class MineManager
{
    
    public MineManager()
    {
        
    }
    
    // returns true / false if MineAction is successful / not
    public bool Mine(BlockCoordinateInformation blockPointedCoords)
    {        
        if (blockPointedCoords == null) return false;

        Block blockPointed = blockPointedCoords.GetBlockIfExists();
        if (blockPointed == null) {
            Debug.LogWarning("BUG Want to break a air");
            return false;
        }

        if (blockPointed.GetBlockType().GetName() == "bedrock")
        {
            return false;
        }
        
        Chunk chunkPointed = blockPointed.GetChunk();
        Vector3Int blockIndex = blockPointed.GetInChunkIndex();
        int x = blockIndex.x;
        int y = blockIndex.y;
        int z = blockIndex.z;
        chunkPointed.SetBlock(x,y,z,null);
        return true;
    }
}
