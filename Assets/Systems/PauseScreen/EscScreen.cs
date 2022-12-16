using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscScreen : MonoBehaviour
{
    GameObject pauseScreen;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pauseScreen = player.transform.Find("UI").Find("PauseScreen").gameObject;
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
    }

    public void OnClickBackToGame() {
        pauseScreen.SetActive(false);
        player.GetComponent<PointingTo>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;	
    }

    public void OnClickSaveAndQuit() {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
