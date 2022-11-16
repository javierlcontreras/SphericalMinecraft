public class Block {
    int xInChunk, yInChunk, height; // relative to chunk
    public BlockType type;

    public Block(int _xInChunk, int _yInChunk, int _height, BlockType _type) {
        xInChunk = _xInChunk;
        yInChunk = _yInChunk;
        height = _height;
        type = _type;
    }
}