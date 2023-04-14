using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class EscScreen : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private PointingTo _pointingTo;

    public void MyUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            bool active = pauseScreen.activeSelf;
            if (active) {
                OnClickBackToGame();
            }
            else {
                _pointingTo.enabled = false;
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
        _pointingTo.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;	
    }

    public void OnClickSaveAndQuit() {
        SaveSystemManager save = GameObject.Find("SaveSystemManager").GetComponent<SaveSystemManager>();
        save.SaveGame();
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
