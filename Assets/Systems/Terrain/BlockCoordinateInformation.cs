using UnityEngine;

public class BlockCoordinateInformation
{
    private PlanetTerrain planet;
    private Vector3Int chunkCoord;
    private Vector3Int blockCoord;

    private SphericalBlockAdjacencyCalculator sphericalBlockAdjacencyCalculator;
    public BlockCoordinateInformation(Vector3Int _blockCoord, Vector3Int _chunkCoord, PlanetTerrain _planet)
    {
        blockCoord = _blockCoord;
        chunkCoord = _chunkCoord;
        planet = _planet;

        sphericalBlockAdjacencyCalculator = new SphericalBlockAdjacencyCalculator(planet);
    }

    public PlanetTerrain GetPlanet()
    {
        return planet;
    }
    public Vector3Int GetBlockCoords()
    {
        return blockCoord;
    }

    public Vector3Int GetChunkCoords()
    {
        return chunkCoord;
    }

    public Block GetBlockIfExists()
    {
        Chunk chunk = planet.GetChunk(chunkCoord);
        if (chunk == null) return null;
        Block block = chunk.GetBlock(blockCoord);
        return block;
    }

    public Chunk GetChunkIfExists()
    {
        return planet.GetChunk(chunkCoord);
    }

    public BlockAdjacency BlockNextToMe(Vector3Int pointingTo)
    {
        return sphericalBlockAdjacencyCalculator.BlockNextToMe(blockCoord, chunkCoord, pointingTo);
    }

    public Vector3 PositionInPlanetReference()
    {
        // TODO(jlcontreras): Reimplement this without actually creating a block would be nice
        Chunk chunk = planet.GetChunk(chunkCoord);
        Block block = new Block(blockCoord, BlockTypeEnum.GetByName("air"), chunk);
        return block.GetBlockPosition();
    }

    public Vector3 PositionInGlobalReference()
    {
        // TODO(jlcontreras): Reimplement this without actually creating a block would be nice
        Chunk chunk = planet.GetChunk(chunkCoord);
        Block block = new Block(blockCoord, BlockTypeEnum.GetByName("air"), chunk);
        return block.GetBlockGlobalPosition();
    }

    public float DistanceTo(Vector3 point)
    {
        Vector3 position = PositionInPlanetReference();
        return (position - point).magnitude;
    }
    
    
}
