using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class IndexScreenClickManager : MonoBehaviour
{
    [SerializeField] private GameObject singleplayerScreen;
    [SerializeField] private GameObject multiplayerScreen;
    [SerializeField] private GameObject usernameScreen;
    [SerializeField] private TMP_Text welcomeUsernameText;

    private string userId;
    private string defaultWelcomeText;
    public void Start()
    {
        defaultWelcomeText = welcomeUsernameText.text;
        userId = PlayerPrefs.GetString("userId");
        if (userId == null || userId == "")
        {
            OnChangeUsernameClick();
        }
        else
        {
            modifyWelcomeMessage();
        }
    }

    public void SetUserId(string _userId)
    {
        
        userId = _userId;
        PlayerPrefs.SetString("userId", userId);
        modifyWelcomeMessage();
    }

    private void modifyWelcomeMessage()
    {
        string welcome = welcomeUsernameText.text;
        welcome = welcome.Replace("<username>", userId);
        welcomeUsernameText.text = welcome;
    }
    
    public void OnSingleplayerClick() {
        
        transform.gameObject.SetActive(false);
        singleplayerScreen.SetActive(true);
    }
    public void OnMultiplayerClick() {
       
        transform.gameObject.SetActive(false);
        multiplayerScreen.SetActive(true);
    }

    public void OnChangeUsernameClick()
    {
        welcomeUsernameText.text = defaultWelcomeText;
        PlayerPrefs.DeleteKey("userId");
        transform.gameObject.SetActive(false);
        usernameScreen.SetActive(true);
    }
    
}
