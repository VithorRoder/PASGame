using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public Transform roomListContent; // Reference to the Content object in the ScrollView
    public GameObject roomButtonPrefab; // Prefab for room buttons

    private List<GameObject> roomButtons = new List<GameObject>(); // List of buttons for cleanup

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Clear old buttons before updating the list
        foreach (GameObject button in roomButtons)
        {
            Destroy(button);
        }
        roomButtons.Clear();

        // Create a button for each available room
        foreach (RoomInfo room in roomList)
        {
            if (!room.RemovedFromList) // Avoid listing closed rooms
            {
                GameObject roomButton = Instantiate(roomButtonPrefab, roomListContent);

                // Room name (Aligned to the left)
                Text roomNameText = roomButton.transform.Find("RoomNameText").GetComponent<Text>();
                roomNameText.text = room.Name;

                // Player count (Aligned to the right and green)
                Text playerCountText = roomButton.transform.Find("PlayerCountText").GetComponent<Text>();
                playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers}";
                playerCountText.color = Color.green; // Set text color to green

                // Set click event to join the room
                roomButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name));

                // Add the button to the list for future cleanup
                roomButtons.Add(roomButton);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
