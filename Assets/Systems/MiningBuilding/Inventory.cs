using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    InventorySlot[] slots = new InventorySlot[9];
    int selectedSlotIndex = 0;
    ToolbarManager toolbarmanager;

    void Awake()
    {
        toolbarmanager = gameObject.transform.Find("UI/HUD/Toolbar").GetComponent<ToolbarManager>();
        slots[0] = new InventorySlot(64, BlockTypeEnum.GetByName("grass"));
        slots[1] = new InventorySlot(64, BlockTypeEnum.GetByName("dirt"));
        slots[2] = new InventorySlot(64, BlockTypeEnum.GetByName("sand"));
        slots[3] = new InventorySlot(32, BlockTypeEnum.GetByName("stone"));
        slots[4] = new InventorySlot(64, BlockTypeEnum.GetByName("cobblestone"));
        slots[5] = new InventorySlot(64, BlockTypeEnum.GetByName("gravel"));
        slots[6] = new InventorySlot(64, BlockTypeEnum.GetByName("wood"));
        slots[7] = new InventorySlot(64, BlockTypeEnum.GetByName("leaves"));
    }

    public InventorySlot GetSlot(int index) {
        return slots[index];
    }

    public int GetSelectedSlotIndex() {
        return selectedSlotIndex;
    }

    void Update()
    {
        if(Input.inputString != ""){
            int number;
            bool is_a_number = Int32.TryParse(Input.inputString, out number);
            if (is_a_number && number >= 1 && number < 10){
                selectedSlotIndex = number-1;
                toolbarmanager.RecolorSlots();
            }
        }
    }
}
