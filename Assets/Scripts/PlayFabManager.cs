using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayFabManager : MonoBehaviour
{
    #region Variables
    private string username;
    private string password;
    private string email; 
    private string playFabPlayerIdCache;
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
        var request = new LoginWithPlayFabRequest { Username = username, Password = password};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnPlayFabError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        playFabPlayerIdCache = result.PlayFabId;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayFabSignUp()
    {
        var request = new RegisterPlayFabUserRequest { Username = username, Email = email, Password = password };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnSignUpSuccess, OnPlayFabError);
    }
    private void OnSignUpSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Sign-up Success. Your ID is " + result.PlayFabId);
    }

    public void SetUsername(string username)
    {
        this.username = username;
    }

    public void SetEmail(string email)
    {
        this.email = email;
    }

    public void SetPassword(string password)
    {
        this.password = password;
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
            try
            {
                PlayFabClientAPI.GetPhotonAuthenticationToken(photonRequest, AuthWithPhoton, OnPlayFabError);
            }
            catch
            {
                Debug.Log("Could not get authentication token, please log in!");
            }
        }
    }

    private void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        Debug.Log("Photon token acquired: " + result.PhotonCustomAuthenticationToken + "  Authentication complete.");
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        customAuth.AddAuthParameter("username", playFabPlayerIdCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = customAuth;
        PhotonManager.ConnectToPhotonService(username);
    }
    #endregion

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong");
        Debug.LogError(error.GenerateErrorReport());
    }

}

