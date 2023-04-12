using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class CreateNewWorldScreenClickManager : MonoBehaviour
{
    [SerializeField] private TMP_Text worldNameTextbox;
    [SerializeField] private TMP_Text seedTextbox;
    [SerializeField] private GameObject singleplayerScreen;
    
    public void OnClickCreateNewWorld() {
        string worldName = worldNameTextbox.text;
        string seed = worldNameTextbox.text;
        
        PlayerPrefs.SetInt("newWorld", 1);
        PlayerPrefs.SetString("worldName", worldName);
        PlayerPrefs.SetString("seed", seed);
        PlayerPrefs.SetString("mode", "host");
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void OnClickBack()
    {
        singleplayerScreen.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
