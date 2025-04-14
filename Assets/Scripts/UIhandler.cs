using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIhandler : MonoBehaviourPunCallbacks
{
    public InputField createRoomTF;
    public InputField joinRoomTF;
    public GameObject joiningRoom;
    public GameObject creatingRoom;
    public GameObject failedFindRoom;
    public GameObject waitingRoom;
    public GameObject warningPanel;
    public Text warningText;

    public void OnClick_JoinRoom()
    {
        if (!ValidateSelection()) return;

        failedFindRoom.SetActive(false);
        creatingRoom.SetActive(false);
        joiningRoom.SetActive(true);
        PhotonNetwork.JoinRoom(joinRoomTF.text, null);
        StartCoroutine(WaitForJoin());
    }

    public void OnClick_CreateRoom()
    {
        if (!ValidateSelection()) return;

        failedFindRoom.SetActive(false);
        joiningRoom.SetActive(false);
        creatingRoom.SetActive(true);
        PhotonNetwork.CreateRoom(createRoomTF.text, new RoomOptions { MaxPlayers = 6 }, null);
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

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined Successfully!");
        waitingRoom.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join the room: " + message);
        ShowWarning("Failed to join the room.");
    }

    public bool ValidateSelection()
    {
        GameObject nameObj = GameObject.Find("SelectedNameB");
        GameObject charObj = GameObject.Find("CharacterSelectedB");

        string selectedName = "";
        string selectedChar = "";

        if (nameObj != null && nameObj.GetComponent<Text>() != null)
            selectedName = nameObj.GetComponent<Text>().text;

        if (charObj != null && charObj.GetComponent<Text>() != null)
            selectedChar = charObj.GetComponent<Text>().text;

        if (string.IsNullOrEmpty(selectedName) || string.IsNullOrEmpty(selectedChar))
        {
            ShowWarning("You need to select a name or a character to play!");
            return false;
        }

        return true;
    }

    public void ShowWarning(string message, float duration = 4f)
    {
        StopAllCoroutines();
        StartCoroutine(ShowWarningRoutine(message, duration));
    }

    private IEnumerator ShowWarningRoutine(string message, float duration)
    {
        if (warningPanel != null && warningText != null)
        {
            warningText.text = message;
            warningPanel.SetActive(true);
            yield return new WaitForSeconds(duration);
            warningPanel.SetActive(false);
        }
    }

    public void CloseWarningPanel()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}
