using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject configPanel;

    public void StartBotton()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("home");

    }
    

    public void ShowconfigPanel()
    {
        configPanel.SetActive(true);
    }

    public void HideConfigPanel()
    {
        configPanel.SetActive(false);
}

    public void ItemPageBotton() 
    {
        Debug.Log("Itempage");
        SceneManager.LoadScene("Item");
    }

    public void StatusPageBotton()
    {
        Debug.Log("Statuspage");
        SceneManager.LoadScene("Status");
    }

    public void ScenarioPageBotton()
    {
        Debug.Log("Scenariopage");
        SceneManager.LoadScene("ScenarioTest");
    }

    public void SettingBotton()
    {
        Debug.Log("Settingpage");
        SceneManager.LoadScene("Setting");
    }

    public void BackbumerBotton()
    {
        Debug.Log("Backnumberpage");
        SceneManager.LoadScene("Backnumber");
    }

    public void TitleBotton()
    {
        Debug.Log("Titlepage");
        SceneManager.LoadScene("Title");
    }

    public void HomeBotton()
    {
        Debug.Log("Homepage");
        SceneManager.LoadScene("home");
    }
}
