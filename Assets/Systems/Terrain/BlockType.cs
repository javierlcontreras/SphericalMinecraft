using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockType
{
    [SerializeField] private string typeName;
    [SerializeField] private bool stackable = true;
    [SerializeField] private bool air = false;
    [SerializeField] private bool unimplemented = false;
    [SerializeField] private int stackSize = 64;

    [SerializeField] private Vector2Int atlasCoordinateUp;
    [SerializeField] private Vector2Int atlasCoordinateDown;
    [SerializeField] private Vector2Int atlasCoordinateRight;
    [SerializeField] private Vector2Int atlasCoordinateLeft;
    [SerializeField] private Vector2Int atlasCoordinateForward;
    [SerializeField] private Vector2Int atlasCoordinateBack;

    public BlockType()
    {
        
    }
    
    public int GetStackSize()
    {
        return stackSize;
    }

    public string GetName()
    {
        return typeName;
    }

    public bool IsAir()
    {
        return air;
    }

    public bool IsStackable()
    {
        return stackable;
    }
    
    public Vector2Int GetAtlasCoord(string side)
    {
        if (unimplemented)
        {
            Debug.LogWarning("This block: " + typeName + " is unimplemented");
        }
        if (side == "up") return atlasCoordinateUp;
        if (side == "down") return atlasCoordinateDown;
        if (side == "right") return atlasCoordinateRight;
        if (side == "left") return atlasCoordinateLeft;
        if (side == "forward") return atlasCoordinateForward;
        if (side == "back") return atlasCoordinateBack;
        return Vector2Int.zero;
    }
}