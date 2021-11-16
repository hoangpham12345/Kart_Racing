using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject modeSelectionMenu;
    public GameObject singlePlayerPanel;
    public GameObject multiplayerPanel;
    public GameObject gamePanel;
    private GameObject[] menus;

    public void Start()
    {
        menus = new GameObject[] { mainMenu, settingsMenu, modeSelectionMenu };
    }

    public void Playgame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void ToMainMenu()
    {
        SwitchToMenu(mainMenu);
    }

    public void ToSettingsMenu()
    {
        SwitchToMenu(settingsMenu);
    }

    public void ToModeSelectionMenu()
    {
        SwitchToMenu(modeSelectionMenu);
    }

    public void ToSinglePlayer()
    {
        OpenGamePanel();
        singlePlayerPanel.SetActive(true);
        multiplayerPanel.SetActive(false);
    }

    public void ToMultiplayer()
    {
        OpenGamePanel();
        multiplayerPanel.SetActive(true);
        singlePlayerPanel.SetActive(false);
    }

    public void OpenGamePanel()
    {
        gamePanel.SetActive(true);
    }

    public void CloseGamePanel()
    {
        gamePanel.SetActive(true);
    }

    private void SwitchToMenu(GameObject menu)
    {
        foreach(GameObject m in menus)
        {
            m.SetActive(false);
        }
        menu.SetActive(true);
        gamePanel.SetActive(false);
    }


}
