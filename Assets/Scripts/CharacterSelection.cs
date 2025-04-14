using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CharacterSelection : MonoBehaviour
{
    public Image characterImage;
    public Sprite[] characterSprites;
    public string[] characterNames;
    public int currentIndex = 0;
    public Text textFieldCharacterName;

    public void UpdateCharacter(int index)
    {
        characterImage.sprite = characterSprites[index];
        textFieldCharacterName.text = characterNames[index];
        currentIndex = index;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "charIndex", index }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Debug.Log("Novo charIndex salvo: " + index);

        WaitingRoomManager waitingRoom = FindObjectOfType<WaitingRoomManager>();
        if (waitingRoom != null)
        {
            waitingRoom.UpdatePlayerListUI();
        }
    }
}