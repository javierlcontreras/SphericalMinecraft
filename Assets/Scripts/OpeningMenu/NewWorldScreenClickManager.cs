using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewWorldScreenClickManager : MonoBehaviour
{
    private string userId;
    [SerializeField] private GameObject indexScreen;
    
    public void SetUserId(string _userId)
    {
        userId = _userId;
    }

    public void OnClickCreateNewWorld() {
        string worldName = transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        string seed = transform.Find("InputSeed").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "")
        {
            // TODO this is only for debugging. In game, setting no name should be an error
            worldName = "testworld";
        }
        PlayerPrefs.SetInt("newWorld", 1);
        PlayerPrefs.SetString("userId", userId);
        PlayerPrefs.SetString("worldName", worldName);
        PlayerPrefs.SetString("seed", seed);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }        
    public void OnClickBack() {
        indexScreen.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
