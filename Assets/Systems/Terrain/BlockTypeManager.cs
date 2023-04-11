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

    public static BlockTypeManager Instance { get; private set; }
    private void Awake() { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
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
