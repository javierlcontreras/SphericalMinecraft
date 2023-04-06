using System.Linq;
using UnityEngine;

public class BlockAdjacency {
    Block[] list;

    public BlockAdjacency()
    {
        list = new Block[0];
    }
    public BlockAdjacency(Block block) {
        if (block == null)
        {
            Debug.LogWarning("Creating a blockadjacency to an inexistent block");
        }
        list = new Block[1] {
            block
        };
    }

    public BlockAdjacency(Block[] _list) {
        list = _list;
    }

    public Block ClosestTo(Vector3 pointInGlobal)
    {
        float minDistance = list.Min(block => block.DistanceTo(pointInGlobal));
        return list.First(block => block.DistanceTo(pointInGlobal) == minDistance);
    }
    
    public bool IsAnyAir ()
    {
        if (list.Length == 0) return true;
        foreach (Block block in list) {
            if (block == null || block.GetBlockType().GetName() == "air") return true; 
        }
        return false;
    }
}