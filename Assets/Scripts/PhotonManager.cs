using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //Singeleton Implementation
    public static PhotonManager instance = null;

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

    public static void ConnectToPhotonService(string nickName)
    {
        Debug.Log("Try to connect to Photon service");
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.LocalPlayer.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }


    public override void OnConnected()
    {
        Debug.Log("We connected to the Internet");
    }

    public override void OnConnectedToMaster()
    {
        // base.OnConnectedToMaster();
        Debug.Log(PhotonNetwork.LocalPlayer.ToStringFull());
    }

    public void DisconnectFromPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Try to disconnect from Photon service");
            PhotonNetwork.Disconnect();
        }
    }

}
