using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //Singeleton Implementation
    public static PhotonManager instance = null;

    [SerializeField]
    private byte maxPlayers = 4;

    public TMP_Text roomMessageInputField;

    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    public TMP_Text roomInfo;

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

    public void DisconnectFromPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Try to disconnect from Photon service");
            PhotonNetwork.Disconnect();
        }
    }

    #region Lobby

    public override void OnConnected()
    {
        Debug.Log("We connected to the Internet");
    }

    public override void OnConnectedToMaster()
    {
        // base.OnConnectedToMaster();
        Debug.Log(PhotonNetwork.LocalPlayer.ToStringFull());
        JoinLobby();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(customLobby);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
    #endregion

    #region Room manupulation
    public void CreateNewRoom()
    {
        string roomMessage = roomMessageInputField.text;
        if (string.IsNullOrEmpty(roomMessage))
        {
            // default roomMessage
            roomMessage = "Come and play!";
        }
        System.Random random = new System.Random();
        string roomID = random.Next(0, 100).ToString();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        string[] roomPropsInLobby = { "message" };

        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {{ "message", roomMessage }};

        roomOptions.CustomRoomProperties = customRoomProperties;
        roomOptions.CustomRoomPropertiesForLobby = roomPropsInLobby;

        PhotonNetwork.CreateRoom(roomID, roomOptions);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "joined to " + PhotonNetwork.CurrentRoom.Name);
        roomInfo.text = "Room ID:" + PhotonNetwork.CurrentRoom.Name + "\t" + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        // We will create new room with default message
        CreateNewRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    #endregion
}
