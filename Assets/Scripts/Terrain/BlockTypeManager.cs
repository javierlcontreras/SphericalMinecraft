using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;

public class BlockTypeManager : MonoBehaviour
{
    [SerializeField]
    private List<BlockType> blockTypesList;
    private Dictionary<string, BlockType> blockTypes;

    public static BlockTypeManager Singleton { get; private set; }
    private void Awake() { 
        if (Singleton != null && Singleton != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Singleton = this; 
        }

        // Init dictioary
        blockTypes = new Dictionary<string, BlockType>();
        foreach (BlockType blockType in blockTypesList)
        {
            blockTypes[blockType.GetName()] = blockType;
        }
    }

    public BlockType GetByName(string name)
    {
        return blockTypes[name];
    }

    public BlockType GetByIndex(int index)
    {
        Debug.LogWarning("Getting block by id, this should only be done in testing");
        return blockTypes.Values.ToArray()[index];
    }
}
