using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;

    private CharacterSelection characterSelection;

    void Start()
    {
        characterSelection = FindObjectOfType<CharacterSelection>();
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int playerIndex = characterSelection.currentIndex;
        
        int spawnIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1; 

        if (spawnIndex >= spawnPoints.Length)
        {
            spawnIndex = spawnIndex % spawnPoints.Length;
        }

        Vector3 spawnPosition = spawnPoints[spawnIndex].position;
        PhotonNetwork.Instantiate(playerPrefabs[playerIndex].name, spawnPosition, Quaternion.identity);
    }
}