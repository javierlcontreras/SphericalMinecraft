public class Block {
    public int xInChunk {get;}
    public int yInChunk {get;}
    public int height {get;}  // relative to chunk
    public BlockType type;

    public Block(int _xInChunk, int _yInChunk, int _height, BlockType _type) {
        xInChunk = _xInChunk;
        yInChunk = _yInChunk;
        height = _height;
        type = _type;
    }
}