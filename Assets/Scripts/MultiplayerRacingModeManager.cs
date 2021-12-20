using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerRacingModeManager : MonoBehaviour
{
    public GameObject[] PlayerPrefabs;
    public Transform[] InstantiatePositions;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerPlayerConfig.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = InstantiatePositions[actorNumber - 1].position;

                PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);

            }


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
