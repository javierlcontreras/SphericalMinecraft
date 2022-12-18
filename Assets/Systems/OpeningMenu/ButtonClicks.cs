using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonClicks : MonoBehaviour
{
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
        indexScreen.SetActive(false);
        newWorldScreen.SetActive(true);
    }
    public void OnClickLoadWorld() {
        indexScreen.SetActive(false);
        loadWorldScreen.SetActive(true);
    }
    public void OnClickCreateNewWorld() {
        string worldName = newWorldScreen.transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        string seed = newWorldScreen.transform.Find("InputSeed").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "") return;
        PlayerPrefs.SetInt("newWorld", 1);
        PlayerPrefs.SetString("worldName", worldName);
        PlayerPrefs.SetString("seed", seed);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    public void OnClickLoadLoadWorld() {
        string worldName = loadWorldScreen.transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "") return;
        PlayerPrefs.SetInt("newWorld", 0);
        PlayerPrefs.SetString("worldName", worldName);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    public void OnClickBack() {
        indexScreen.SetActive(true);
        newWorldScreen.SetActive(false);
        loadWorldScreen.SetActive(false);
    }

}
