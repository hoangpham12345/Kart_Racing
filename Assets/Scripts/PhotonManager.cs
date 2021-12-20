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

    private string roomMessage;
    private string roomID;

    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    // Panels to manipulate between lobby and room
    public GameObject multiplayerLobby;
    public GameObject multiplayerRoom;

    private string roomInfo;

    // player prefab and its container in the room
    public GameObject playerListPrefab;
    public GameObject playerListContainer;

    public GameObject startGameButton;

    private Dictionary<int, GameObject> playerListGameObjects;

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

    public void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
            multiplayerLobby.SetActive(false);
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
        Debug.Log("Try to join the lobby");
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
        Debug.Log("Joined lobby");
        cachedRoomList.Clear();
        multiplayerLobby.SetActive(true);
        multiplayerRoom.SetActive(false);
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
    public void SetRoomMessage(string roomMessage)
    {
        this.roomMessage = roomMessage;
    }

    public void SetRoomID(string roomID)
    {
        this.roomID = roomID;
    }

    public void CreateNewRoom()
    {
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
        roomMessage = "";
    }

    public void JoinRoomWithID()
    {
        if (string.IsNullOrEmpty(roomID))
        {
            Debug.Log("You must enter the roomID");
        } else
        {
            PhotonNetwork.JoinRoom(roomID);
        }

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
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " " + "joined to " + PhotonNetwork.CurrentRoom.Name);
        roomInfo = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        multiplayerLobby.SetActive(false);
        multiplayerRoom.SetActive(true);

        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = roomInfo;

        // Update Player list

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(playerListPrefab);
            playerListGameObject.transform.SetParent(playerListContainer.transform);
            playerListGameObject.transform.localScale = Vector3.one;
            playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);

            playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
        }

        startGameButton.SetActive(false);
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // Create new player game object
        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContainer.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGameObject;
        if (playerListGameObjects.TryGetValue(targetPlayer.ActorNumber, out playerListGameObject))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(MultiplayerPlayerConfig.PLAYER_READY, out isPlayerReady))
            {

                playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);

            }
        }
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Update room's title
        GameObject roomTitleGO = GetChildWithName(multiplayerRoom, "RoomTitle");
        TMP_Text roomTitle = roomTitleGO.GetComponent<TMP_Text>();
        roomTitle.text = "Room ID:" + PhotonNetwork.CurrentRoom.Name + " " + "Players:" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
        startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnLeftRoom()
    {
        //BackToLobby();

        multiplayerLobby.SetActive(true);
        multiplayerRoom.SetActive(false);
        foreach (GameObject playerListGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.SetActive(CheckPlayersReady());
        }
    }
    #endregion


    public void BackToLobby()
    { 
        PhotonNetwork.LeaveRoom();

    }

    public void StartMultiplayerGame()
    {
        PhotonNetwork.LoadLevel("Map2Multiplayer");
    }

    #region Supplemental code
    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    private bool CheckPlayersReady()
    {

        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            return false;
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(MultiplayerPlayerConfig.PLAYER_READY, out isPlayerReady))
            {

                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    #endregion

}
