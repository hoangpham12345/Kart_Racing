using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;

public class PlayFabManager : MonoBehaviour
{
    #region Variables
    private string _username;
    private string _password;
    private string _playFabPlayerIdCache;
    #endregion

    //Singeleton Implementation
    public static PlayFabManager instance = null;

    private void Awake()
    {
        //check if instance already exists
        if (instance == null)
        {
            instance = this;
        }

        //If intance already exists and it is not !this!
        else if (instance != this)
        {
            //Then, destroy this. This enforces our singletton pattern, meaning that there can only ever be one instance of a GameManager
            Destroy(gameObject);

        }

        //To not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }

    #region PlayFab Authentication
    public void PlayFabLogin()
    {
        var request = new LoginWithPlayFabRequest { Username = _username, Password = _password};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnPlayFabError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        _playFabPlayerIdCache = result.PlayFabId;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetUsername(string username)
    {
        _username = username;
    }

    public void SetPassword(string password)
    {
        _password = password;
    }

    #endregion

    #region Get Photon token
    public void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("PlayFab authenticated. Requesting photon token...");

            GetPhotonAuthenticationTokenRequest photonRequest = new GetPhotonAuthenticationTokenRequest();
            photonRequest.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
            PlayFabClientAPI.GetPhotonAuthenticationToken(photonRequest, AuthWithPhoton, OnPlayFabError);
        }
    }

    private void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        Debug.Log("Photon token acquired: " + result.PhotonCustomAuthenticationToken + "  Authentication complete.");
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = customAuth;
        PhotonManager.ConnectToPhotonService(_username);
    }
    #endregion

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong");
        Debug.LogError(error.GenerateErrorReport());
    }

}

