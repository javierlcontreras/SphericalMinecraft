using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MultiplayerScreenClickManager : MonoBehaviour
{
    private string userId;
    [SerializeField] private GameObject indexScreen;

    public void SetUserId(string _userId)
    {
        userId = _userId;
    }
    
    public void OnClickLoadLoadWorld() {
        string worldName = transform.Find("InputWorldName").gameObject.GetComponent<TMP_InputField>().text;
        if (worldName == "")
        {
            // TODO this is only for debugging. In game, setting no name should be an error
            worldName = "testworld";
        }
        PlayerPrefs.SetInt("newWorld", 0);
        PlayerPrefs.SetString("worldName", worldName);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    } 
    public void OnClickBack() {
        indexScreen.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}