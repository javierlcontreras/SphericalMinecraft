using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    InventorySlot[] slots = new InventorySlot[9];
    void Awake()
    {
        slots[0] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("grass"));
        slots[1] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("dirt"));
        slots[2] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("sand"));
        slots[3] = new InventorySlot(32, BlockTypeEnum.GetBlockTypeByName("stone"));
        slots[4] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("cobblestone"));
        slots[5] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("gravel"));
        slots[6] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("wood"));
        slots[7] = new InventorySlot(64, BlockTypeEnum.GetBlockTypeByName("leaves"));
    }

    public InventorySlot GetSlot(int index) {
        return slots[index];
    }
}
