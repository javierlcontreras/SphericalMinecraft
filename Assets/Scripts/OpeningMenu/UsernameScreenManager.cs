using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class UsernameScreenManager : MonoBehaviour
{
    [SerializeField] private TMP_Text inputTextBoxUserId;
    [SerializeField] private GameObject indexScreen;
    [SerializeField] private GameObject wrongUsernameWarning;
    
    public void Start()
    {
        string userId = PlayerPrefs.GetString("userId");
        if (userId != "")
        {
            AcceptUsername(userId);
        }
    }

    public void OnAcceptClick()
    {
        string userId = inputTextBoxUserId.text.Substring(0, inputTextBoxUserId.text.Length-1);
        OnEndEditUsernameTextBox(userId);
    }
    
    public void OnEndEditUsernameTextBox(string userId)
    {
        if (IsValidUsername(userId))
        {
            AcceptUsername(userId);
        }
        else
        {
            RejectUsername();
        }
    }

    bool IsValidUsername(string userId)
    {
        if (userId.Length < 4) return false;
        if (userId.Length > 20) return false;
        
        return userId.All(char.IsLetterOrDigit);
    }

    void RejectUsername()
    {
        wrongUsernameWarning.SetActive(true);
    }
    
    private void AcceptUsername(string userId)
    {
        wrongUsernameWarning.SetActive(false);
        transform.gameObject.SetActive(false);
        indexScreen.SetActive(true);
        indexScreen.GetComponent<IndexScreenClickManager>().SetUserId(userId);
    }
}
