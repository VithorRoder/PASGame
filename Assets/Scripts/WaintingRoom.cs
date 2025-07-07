using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Button startButton;
    public Button exitButton;
    public Text roomNameText;
    public GameObject[] playerSlots;
    public Sprite[] characterSprites;

    void Start()
    {
        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExitRoom);
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerListUI();
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
        }
        UpdatePlayerListUI();
    }

    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps["isStarted"] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

            PhotonNetwork.LoadLevel(1);
        }
    }

    public void OnExitRoom()
    {
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        if (props.ContainsKey("charIndex"))
        {
            props.Remove("charIndex");
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        GameObject connectedScreen = GameObject.Find("ConnectedScreenS");
        GameObject waitingRoom = GameObject.Find("WaitingRoom");
        GameObject conecting = GameObject.Find("CreatingRoom");

        if (connectedScreen != null) connectedScreen.SetActive(true);
        if (waitingRoom != null) waitingRoom.SetActive(false);
        if (conecting != null) conecting.SetActive(false);

        cleanFields();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("charIndex"))
        {
            Debug.Log("charIndex atualizado para player " + targetPlayer.NickName);
            UpdatePlayerListUI();
        }
    }

    public void cleanFields()
    {
        GameObject obj;

        obj = GameObject.Find("SelectedNameB");
        if (obj != null && obj.GetComponent<Text>() != null)
            obj.GetComponent<Text>().text = "";

        obj = GameObject.Find("CharacterSelectedB");
        if (obj != null && obj.GetComponent<Text>() != null)
            obj.GetComponent<Text>().text = "";

        obj = GameObject.Find("NameTF");
        if (obj != null && obj.GetComponent<InputField>() != null)
            obj.GetComponent<InputField>().text = "";

        obj = GameObject.Find("CreateRoomTF");
        if (obj != null && obj.GetComponent<InputField>() != null)
            obj.GetComponent<InputField>().text = "";

        obj = GameObject.Find("JoinRoomTF");
        if (obj != null && obj.GetComponent<InputField>() != null)
            obj.GetComponent<InputField>().text = "";
    }

    public void UpdatePlayerListUI()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerSlots.Length; i++)
        {
            GameObject slot = playerSlots[i];
            Text nameText = slot.transform.Find("NameText").GetComponent<Text>();
            Image charImage = slot.transform.Find("CharImage").GetComponent<Image>();

            if (i < players.Length)
            {
                Player player = players[i];
                nameText.text = player.NickName;

                if (player.CustomProperties != null &&
                    player.CustomProperties.ContainsKey("charIndex") &&
                    player.CustomProperties["charIndex"] is int charIndex)
                {
                    if (charImage == null)
                    {
                        Debug.LogError("charImage está null no slot " + i);
                    }
                    else if (characterSprites == null || characterSprites.Length == 0)
                    {
                        Debug.LogError("characterSprites não foi atribuído no Inspector ou está vazio!");
                    }
                    else if (charIndex < 0 || charIndex >= characterSprites.Length)
                    {
                        Debug.LogError("charIndex fora do intervalo: " + charIndex);
                    }
                    else
                    {
                        charImage.sprite = characterSprites[charIndex];
                        Debug.Log("Sprite setado: " + characterSprites[charIndex].name + " para player " + player.NickName);
                    }
                }
                else
                {
                    Debug.Log("Player " + player.NickName + " ainda não escolheu personagem");
                    charImage.sprite = null;
                }

                slot.SetActive(true);
            }
            else
            {
                slot.SetActive(false);
            }
        }
    }
}