using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;

public class PlayFabAuthTest : MonoBehaviour
{
    private string playerIDCache = "";

    private void Awake()
    {
        AuthWithPlayFab();
    }

    private void AuthWithPlayFab()
    {
        Debug.Log("PlayFab authenticating using Custom ID...");
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
        request.CreateAccount = true;
        request.CustomId = PlayFabSettings.DeviceUniqueIdentifier;
        PlayFabClientAPI.LoginWithCustomID(request, RequestPhotonToken, OnPlayFabError);
    }

    private void RequestPhotonToken(LoginResult result)
    {
        Debug.Log("PlayFab authenticated. Requesting photon token...");
        playerIDCache = result.PlayFabId;
        GetPhotonAuthenticationTokenRequest photonRequest = new GetPhotonAuthenticationTokenRequest();
        // photonRequest.PhotonApplicationId = "c16fc88c-9fb7-4f3b-83cf-40941c75b339"; // should be static
        photonRequest.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
        PlayFabClientAPI.GetPhotonAuthenticationToken(photonRequest, AuthWithPhoton, OnPlayFabError);
    }

    private void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        Debug.Log("Photon token acquired: " + result.PhotonCustomAuthenticationToken + "  Authentication complete.");
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

        customAuth.AddAuthParameter("username", playerIDCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
        PhotonNetwork.AuthValues = customAuth;

        PhotonManagerTest.ConnectToPhoton();
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError($"[ERROR] | {error.GenerateErrorReport()}");
    }
}
