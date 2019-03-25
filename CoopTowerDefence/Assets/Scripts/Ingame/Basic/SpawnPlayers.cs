using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayers : Photon.MonoBehaviour
{
    public Transform spawnPoint;
    public string characterName;

    public void Start()
    {
        PhotonNetwork.Instantiate(characterName, spawnPoint.position, Quaternion.identity, 0);
    }
}
