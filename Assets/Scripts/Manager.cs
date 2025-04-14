using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Manager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;
    private CharacterSelection characterSelection;

    void Start()
    {
        characterSelection = FindFirstObjectByType<CharacterSelection>();
        SpawnPlayer();
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    void SpawnPlayer()
    {
        int playerIndex = characterSelection.currentIndex;

        int spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;

        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        PhotonNetwork.Instantiate(playerPrefabs[playerIndex].name, spawnPosition, Quaternion.identity);

        Debug.Log("Spawned player " + PhotonNetwork.NickName + " at point " + spawnIndex);
    }
}
