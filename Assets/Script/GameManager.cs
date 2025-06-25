using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager:MonoBehaviour
{
    
  public void StartBotton()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("home");

    }

}
