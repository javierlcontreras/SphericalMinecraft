using UnityEngine;

public class BuildManager
{
    private Transform character;
    private Inventory inventory;
    public BuildManager(Transform _character, Inventory _inventory)
    {
        inventory = _inventory;
        character = _character;
    }
    
    // returns true / false if BuildAction is successful / not 
    public bool Build(BlockCoordinateInformation blockPointedCoords)
    {
        
        if (blockPointedCoords == null) return false;
        if (blockPointedCoords.GetBlockIfExists() != null)
        {
            Debug.LogWarning("BUG Want to place something in a non-air");
            return false;
        }

        Chunk chunkPointed = blockPointedCoords.GetChunkIfExists();
        if ((blockPointedCoords.PositionInGlobalReference() - character.position).magnitude < 1.5f) {
           /* Vector3Int blockIndex = blockPointedCoords.GetBlockCoords();
            int x = blockIndex.x;
            int y = blockIndex.y;
            int z = blockIndex.z;
            chunkPointed.SetBlock(x,y,z,null);*/
            return false;
        }

        BlockType type = inventory.GetSlot(inventory.GetSelectedSlotIndex()).GetBlockType();
        Vector3Int blockCoords = blockPointedCoords.GetBlockCoords();
        chunkPointed.SetBlock(blockCoords, new Block(blockCoords, type, chunkPointed));
        return true;
    }

}
