using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneHandler : MonoBehaviour
{
    public GameOptions gameOptions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(gameOptions.GetMapName());
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainScene");
    }
}
