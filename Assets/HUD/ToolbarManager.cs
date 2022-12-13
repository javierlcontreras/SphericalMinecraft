using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ToolbarManager : MonoBehaviour
{
    GameObject[] slots;
    int selectedSlot = 0;
    Color originalColor;

    GameObject player;
    Inventory inventory;

    public Texture2D textureAtlasFile;

    void Start()
    {
        slots = new GameObject[9];
        for (int i=0; i<9; i++) {
            slots[i] = GameObject.Find("Slot " + (i+1));
        }
        originalColor = new Color(1f, 1f, 1f, 0.3f);
        player = GameObject.Find("Player");
        inventory = player.GetComponent<Inventory>();
        RecolorSlots();
        FillSlotsWithInventoryItems();
    }

    void Update()
    {
        if(Input.inputString != ""){
            int number;
            bool is_a_number = Int32.TryParse(Input.inputString, out number);
            if (is_a_number && number >= 1 && number < 10){
                selectedSlot = number-1;
                RecolorSlots();
            }
        }
    }

    void RecolorSlots() {
        for (int i=0; i<9; i++) {
            slots[i].GetComponent<Image>().color = originalColor;
        }
        slots[selectedSlot].GetComponent<Image>().color = Color.white;
    }

    void FillSlotsWithInventoryItems() {
        for (int i=0; i<9; i++) {
            InventorySlot slotItem = inventory.GetSlot(i);
            if (slotItem == null) {
                slots[i].transform.Find("BlockImage").gameObject.SetActive(false);
            }
            else {
                Vector2Int atlasCoord = slotItem.GetBlockType().GetAtlasCoord("forward");
                float s = TerrainGenerationConstants.GetTextureBlockSize() * textureAtlasFile.width;
                float x = atlasCoord.x*s;
                float y = textureAtlasFile.width - atlasCoord.y*s;
                Rect rect = new Rect(x,y-s, s, s);
                Sprite sp = Sprite.Create(textureAtlasFile,rect,new Vector2(6.0f,3.0f));
                slots[i].transform.Find("BlockImage").gameObject.SetActive(true);
                slots[i].transform.Find("BlockImage").GetComponent<Image>().sprite = sp;
            }
        }
    }
}
