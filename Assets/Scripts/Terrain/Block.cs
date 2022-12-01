using UnityEngine;

public class Block {
    public Vector3 inChunkPosition;
    public BlockType type;
    public Chunk chunk;

    public Block(Vector3 _inChunkPosition, BlockType _type, Chunk _chunk) {
        inChunkPosition = _inChunkPosition;
        type = _type;
        chunk = _chunk;
    }
}