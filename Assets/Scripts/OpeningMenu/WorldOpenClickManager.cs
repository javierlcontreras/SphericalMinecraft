using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldOpenClickManager : MonoBehaviour
{
    public void OnWorldClick()
    {
        string worldName = gameObject.name;

        if (SaveSystemManager.IsFolderAValidWorld(worldName))
        {
            PlayerPrefs.SetInt("newWorld", 0);
            PlayerPrefs.SetString("worldName", worldName);
            PlayerPrefs.SetString("mode", "host");
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
        else
        {
            // TODO: unimplemented
            Debug.Log("Unimplemented error message: Folder is not a valid world file");
        }
    }
}
