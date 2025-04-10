using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class UIhandler : MonoBehaviourPunCallbacks
{
    public InputField createRoomTF;
    public InputField joinRoomTF;
    public GameObject joiningRoom;
    public GameObject creatingRoom;
    public GameObject failedFindRoom;
    public GameObject waitingRoom;

    public void OnClick_JoinRoom()
    {
        failedFindRoom.SetActive(false);
        creatingRoom.SetActive(false);
        joiningRoom.SetActive(true);
        PhotonNetwork.JoinRoom(joinRoomTF.text, null);

        StartCoroutine(WaitForJoin());
    }

    private IEnumerator WaitForJoin()
    {
        yield return new WaitForSeconds(10f);

        if (!PhotonNetwork.InRoom)
        {
            joiningRoom.SetActive(false);
            creatingRoom.SetActive(false);
            failedFindRoom.SetActive(true);
        }
    }

    public void OnClick_CreateRoom()
    {
        failedFindRoom.SetActive(false);
        joiningRoom.SetActive(false);
        creatingRoom.SetActive(true);
        PhotonNetwork.CreateRoom(createRoomTF.text, new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        print("Room Joined Successfully !");
        waitingRoom.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Failed to join the room." + returnCode + " Message " + message);
    }
}