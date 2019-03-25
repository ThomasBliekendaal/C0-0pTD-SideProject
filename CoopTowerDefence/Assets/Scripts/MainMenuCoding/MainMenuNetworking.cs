using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuNetworking : Photon.MonoBehaviour
{
    public string currentBuildVersion;

    public GameObject mainPanel;
    public InputField roomCodeInput;
    public GameObject roomInputPanel;
    public GameObject roomAlreadyExists;
    public GameObject roomJoinButton;
    public RoomInfo[] rooms;
    public Saving saving;
    public Transform playerLayout;
    public GameObject playerPanel;
    public List<ConnectedPlayerInfo> connectedPlayers = new List<ConnectedPlayerInfo>();
    public GameObject nonMasterPanel;
    public string tempLoadScene;

    public void LoadLevel()
    {
        PhotonNetwork.LoadLevelAsync(tempLoadScene);
    }

    public void OnReceivedRoomListUpdate()
    {
        rooms = PhotonNetwork.GetRoomList();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            if (connectedPlayers[i].name == otherPlayer.NickName)
            {
                Destroy(connectedPlayers[i].panel);
                connectedPlayers.RemoveAt(i);
                break;
            }
        }
        if (PhotonNetwork.isMasterClient)
            nonMasterPanel.SetActive(false);
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        int classIndex = saving.data.classSaves[saving.currentlyPlayingWith].classIndex;
        photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.playerName, classIndex);
    }

    public void OnJoinedRoom()
    {
        int classIndex = saving.data.classSaves[saving.currentlyPlayingWith].classIndex;
        photonView.RPC("AddPlayer", PhotonTargets.All, PhotonNetwork.playerName, classIndex);
        if (!PhotonNetwork.isMasterClient)
            nonMasterPanel.SetActive(true);
    }

    [PunRPC]
    public void AddPlayer(string playerName,int classIndex)
    {
        bool b = true;
        foreach (ConnectedPlayerInfo player in connectedPlayers)
            if (player.name == playerName)
                b = false;

        if (playerName != PhotonNetwork.playerName && b)
        {
            GameObject g = Instantiate(playerPanel, playerLayout);
            ConnectedPlayerInfo info = new ConnectedPlayerInfo(playerName, g);
            g.GetComponent<Image>().sprite = saving.classes[classIndex].image;
            connectedPlayers.Add(info);
        }
        Debug.Log(playerName + " Connected");
    }

    public void EnteredRoomCode()
    {
        bool doesntExist = true;
        if(rooms.Length != 0)
            foreach (RoomInfo room in rooms)
                if (room.Name == roomCodeInput.text)
                    doesntExist = false;
        if (doesntExist)
        {
            PhotonNetwork.JoinOrCreateRoom(roomCodeInput.text, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
            roomInputPanel.SetActive(false);
        }
        else
            roomAlreadyExists.SetActive(true);
    }

    public void StillJoin()
    {
        PhotonNetwork.JoinOrCreateRoom(roomCodeInput.text, new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
        roomInputPanel.SetActive(false);
        roomAlreadyExists.SetActive(false);
    }

    public void CancelJoin()
    {
        roomInputPanel.SetActive(true);
        roomAlreadyExists.SetActive(false);
    }

    public void OpenRoomInputCode()
    {
        roomInputPanel.SetActive(true);
        roomJoinButton.SetActive(false);
    }

    public void CloseRoomInputCode()
    {
        roomInputPanel.SetActive(false);
        roomJoinButton.SetActive(true);
    }

    public void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(currentBuildVersion);
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void OnJoinedLobby()
    {
        mainPanel.SetActive(true);
    }

    [System.Serializable]
    public class ConnectedPlayerInfo
    {
        public string name;
        public GameObject panel;
        public ConnectedPlayerInfo(string _name, GameObject _panel)
        {
            name = _name;
            panel = _panel;
        }
    }
}
