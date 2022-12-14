using UnityEngine;

public class BlockAdjacency {
    Block[] list;

    public BlockAdjacency(Block block) {
        list = new Block[1] {
            block
        };
    }

    public BlockAdjacency(Block[] _list) {
        list = _list;
    }

    public bool IsAnyAir () {
        foreach (Block block in list) {
            if (block == null || block.GetBlockType().GetName() == "air") return true; 
        }
        return false;
    }
}