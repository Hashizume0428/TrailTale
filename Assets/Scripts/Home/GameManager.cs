using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager:MonoBehaviour
{
    
  public void StartBotton()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("home");

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
        SceneManager.LoadScene("Scenario");
    }
}
