using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class SingleplayerScreenClickManager : MonoBehaviour
{
    private string userId;
    [SerializeField] private GameObject indexScreen;
    [SerializeField] private GameObject createWorldScreen;
    [SerializeField] private GameObject worldListItem;
    
    [SerializeField] private GameObject worldListParent;
    [SerializeField] private float marginInWorldList = 1f / 4f;
    public void Start()
    {
        DirectoryInfo directoryInfo = SaveSystemManager.GetWorldFolderDirectory();
        DirectoryInfo[] allWorldFiles = directoryInfo.GetDirectories();
        
        int iter = allWorldFiles.Length;
        RectTransform transformPrefabItem = worldListItem.GetComponent<RectTransform>();
        float height = transformPrefabItem.rect.height;
        
        float width = worldListParent.GetComponent<RectTransform>().sizeDelta.x;
        worldListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(
            width,
            (iter * (1 + marginInWorldList) + marginInWorldList) * height
            );
        
        iter = 0;
        foreach (DirectoryInfo worldFile in allWorldFiles)
        {
            GameObject listItem = Instantiate(worldListItem, worldListParent.transform);
            RectTransform transformListItem = listItem.GetComponent<RectTransform>();
            height = transformListItem.rect.height;
            transformListItem.localPosition += Vector3.down * ((1 + marginInWorldList) * iter * height);
            listItem.GetComponentInChildren<TMP_Text>().text = worldFile.Name;
            listItem.name = worldFile.Name;
            
            iter++;
        }
    }

    public void SetUserId(string _userId)
    {
        userId = _userId;
    }

    public void OnClickCreateNewWorld() {
        transform.gameObject.SetActive(false);
        createWorldScreen.SetActive(true);
    }        
    
    public void OnClickBack() {
        indexScreen.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
