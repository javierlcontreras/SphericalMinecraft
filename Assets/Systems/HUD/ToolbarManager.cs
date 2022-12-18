using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ToolbarManager : MonoBehaviour
{
    GameObject[] slots;
    public Color originalColor;

    GameObject player;
    Inventory inventory;

    public Texture2D textureAtlasFile;

    void Start()
    {
        slots = new GameObject[9];
        for (int i=0; i<9; i++) {
            slots[i] = GameObject.Find("Slot " + (i+1));
        }
        originalColor = new Color(1f, 1f, 1f, 0.2f);
        Transform possiblePlayer = gameObject.transform.parent;
        while (possiblePlayer.gameObject.tag != "Player") {
            possiblePlayer = possiblePlayer.parent;
        }
        player = possiblePlayer.gameObject;
        inventory = player.GetComponent<Inventory>();
        RecolorSlots();
        FillSlotsWithInventoryItems();
    }

    public void RecolorSlots() {
        for (int i=0; i<9; i++) {
            slots[i].GetComponent<Image>().color = originalColor;
        }
        int selectedSlot = inventory.GetSelectedSlotIndex();
        slots[selectedSlot].GetComponent<Image>().color = Color.white;
    }

    public void FillSlotsWithInventoryItems() {
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
