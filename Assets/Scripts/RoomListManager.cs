using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public Transform roomListContent;
    public GameObject roomButtonPrefab;
    private List<GameObject> roomButtons = new List<GameObject>();
    private UIhandler uiHandler;

    void Start()
    {
        uiHandler = FindObjectOfType<UIhandler>();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (GameObject button in roomButtons)
        {
            Destroy(button);
        }
        roomButtons.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList)
            {
                GameObject roomButton = Instantiate(roomButtonPrefab, roomListContent);

                Text roomNameText = roomButton.transform.Find("RoomNameText").GetComponent<Text>();
                roomNameText.text = room.Name;

                Text playerCountText = roomButton.transform.Find("PlayerCountText").GetComponent<Text>();
                playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers}";
                playerCountText.color = Color.green;

                roomButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name));

                roomButtons.Add(roomButton);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        if (uiHandler != null && !uiHandler.ValidateSelection())
            return;

        PhotonNetwork.JoinRoom(roomName);
    }
}