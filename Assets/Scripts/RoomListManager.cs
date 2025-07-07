using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomButtonPrefab;
    private List<GameObject> roomButtons = new List<GameObject>();
    private UIhandler uiHandler;
    public Transform waitingRoomListContent;
    public Transform playingRoomListContent;
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    void Start()
    {
        uiHandler = FindObjectOfType<UIhandler>();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        cachedRoomList = roomList;

        // Limpar listas
        foreach (Transform child in waitingRoomListContent) Destroy(child.gameObject);
        foreach (Transform child in playingRoomListContent) Destroy(child.gameObject);
        roomButtons.Clear();

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) continue;

            // Verifica se a sala já começou
            bool isStarted = false;
            if (room.CustomProperties.ContainsKey("isStarted"))
                isStarted = (bool)room.CustomProperties["isStarted"];

            // Cria botão
            GameObject roomButton = Instantiate(roomButtonPrefab);
            Text roomNameText = roomButton.transform.Find("RoomNameText").GetComponent<Text>();
            Text playerCountText = roomButton.transform.Find("PlayerCountText").GetComponent<Text>();
            Button button = roomButton.GetComponent<Button>();

            roomNameText.text = room.Name;
            playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            if (isStarted)
            {
                // Sala em jogo → botão inativo, cinza, vai para PlayingRooms
                button.interactable = false;
                roomNameText.color = Color.gray;
                playerCountText.color = Color.gray;
                roomButton.transform.SetParent(playingRoomListContent, false);
            }
            else
            {
                // Sala disponível → botão ativo, verde, vai para WaitingRooms
                button.interactable = true;
                roomNameText.color = Color.black;
                playerCountText.color = Color.green;
                string roomNameCopy = room.Name;
                button.onClick.AddListener(() => JoinRoom(roomNameCopy));
                roomButton.transform.SetParent(waitingRoomListContent, false);
            }

            roomButtons.Add(roomButton);
        }
    }

    public void JoinRoom(string roomName)
    {
        if (uiHandler != null && !uiHandler.ValidateSelection())
            return;

        StartCoroutine(DelayedJoin(roomName, 3f));
    }

    private IEnumerator DelayedJoin(string roomName, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Verificação novamente após o delay
        RoomInfo targetRoom = cachedRoomList.Find(room => room.Name == roomName);

        if (targetRoom != null &&
            targetRoom.CustomProperties.ContainsKey("isStarted") &&
            (bool)targetRoom.CustomProperties["isStarted"] == true)
        {
            Debug.LogWarning("Tentou entrar em uma sala que já começou! (após delay)");
            uiHandler.ShowWarning("Essa sala já começou, tente outra.");
            yield break;
        }

        PhotonNetwork.JoinRoom(roomName);
    }
}