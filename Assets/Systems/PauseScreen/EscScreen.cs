using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscScreen : MonoBehaviour
{
    GameObject pauseScreen;
    GameObject hudScreen;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        pauseScreen = player.transform.Find("UI/PauseScreen").gameObject;
        hudScreen = player.transform.Find("UI/HUD").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            bool active = pauseScreen.activeSelf;
            if (active) {
                player.GetComponent<PointingTo>().enabled = true;
                pauseScreen.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
		        Cursor.visible = false;	
            }
            else {
                player.GetComponent<PointingTo>().enabled = false;
                pauseScreen.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
		        Cursor.visible = true;	
            }
        }
        if (Input.GetKeyDown(KeyCode.F2)) { 
            bool active = hudScreen.activeSelf;
            hudScreen.SetActive(!active);
        }
    }

    public void OnClickBackToGame() {
        pauseScreen.SetActive(false);
        player.GetComponent<PointingTo>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;	
    }

    public void OnClickSaveAndQuit() {
        SaveSystemManager save = GameObject.Find("SaveSystemManager").GetComponent<SaveSystemManager>();
        save.SaveGame();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
