using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    #region Variables
    private string _username;
    private string _password;
    #endregion

    #region PlayFab Authentication
    public void PlayFabLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = _username, Password = _password};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnPlayFabError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GetUsername(string username)
    {
        _username = username;
    }

    public void GetPassword(string password)
    {
        _password = password;
    }

    #endregion


    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong");
        Debug.LogError(error.GenerateErrorReport());
    }

}
