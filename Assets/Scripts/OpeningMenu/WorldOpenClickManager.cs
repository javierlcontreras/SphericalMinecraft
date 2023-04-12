using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldOpenClickManager : MonoBehaviour
{
    public void OnWorldClick()
    {
        string worldName = gameObject.name;
        
        PlayerPrefs.SetInt("newWorld", 0);
        PlayerPrefs.SetString("worldName", worldName);
        PlayerPrefs.SetString("mode", "host");
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
