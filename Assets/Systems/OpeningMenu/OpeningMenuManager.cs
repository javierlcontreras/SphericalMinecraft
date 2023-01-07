using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OpeningMenuManager : MonoBehaviour
{
    // TODO: Temporal fixed user id. Show be modificiable from init menu
    string userId;
    GameObject screens;
    GameObject indexScreen;
    GameObject newWorldScreen;
    GameObject loadWorldScreen;
    // Start is called before the first frame update
    void Start()
    {
        
        screens = GameObject.Find("Screens");   
        indexScreen = screens.transform.Find("IndexScreen").gameObject;   
        newWorldScreen = screens.transform.Find("NewWorldScreen").gameObject;   
        loadWorldScreen = screens.transform.Find("LoadWorldScreen").gameObject;   
        indexScreen.SetActive(true);
        newWorldScreen.SetActive(false);
        loadWorldScreen.SetActive(false);
    }

    public void OnClickNewWorld() {
        userId = indexScreen.transform.Find("InputUserId").gameObject.GetComponent<TMP_InputField>().text;
        if (userId == "") return;
        indexScreen.SetActive(false);
        newWorldScreen.SetActive(true);
    }
    public void OnClickLoadWorld() {
        userId = indexScreen.transform.Find("InputUserId").gameObject.GetComponent<TMP_InputField>().text;
        if (userId == "") return;
        indexScreen.SetActive(false);
        loadWorldScreen.SetActive(true);
    }
    public void OnClickCreateNewWorld() {
        string worldName = newWorldScreen.transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        string seed = newWorldScreen.transform.Find("InputSeed").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "") return;
        PlayerPrefs.SetInt("newWorld", 1);
        PlayerPrefs.SetString("userId", userId);
        PlayerPrefs.SetString("worldName", worldName);
        PlayerPrefs.SetString("seed", seed);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    public void OnClickLoadLoadWorld() {
        string worldName = loadWorldScreen.transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "") return;
        PlayerPrefs.SetInt("newWorld", 0);
        PlayerPrefs.SetString("userId", "javierlcontreras");
        PlayerPrefs.SetString("worldName", worldName);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    public void OnClickBack() {
        indexScreen.SetActive(true);
        newWorldScreen.SetActive(false);
        loadWorldScreen.SetActive(false);
    }

}
