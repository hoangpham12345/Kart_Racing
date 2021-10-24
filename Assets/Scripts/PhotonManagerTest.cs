using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManagerTest : MonoBehaviourPunCallbacks
{

    public GameObject[] DisableOnAuth;
    public GameObject[] EnableOnConnect;
    public GameObject[] DisableOnRoom;

    public static void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        string roomId = "RoomID#" + Random.Range(0, 10000).ToString() + Random.Range(0, 10000).ToString();
        PhotonNetwork.JoinOrCreateRoom(roomId, new RoomOptions { IsOpen = true, MaxPlayers = 8, IsVisible = true }, TypedLobby.Default, null);
    }

    public void JoinRoom()
    {

    }

    public override void OnJoinedRoom()
    {
        foreach (GameObject g in DisableOnRoom)
        {
            g.SetActive(false);
        }
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 2, 0), Quaternion.identity, 0, null);
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogWarning("We have authed with photon and CONNECTED!");
        foreach (GameObject g in DisableOnAuth)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in EnableOnConnect)
        {
            g.SetActive(true);
        }
    }

}
