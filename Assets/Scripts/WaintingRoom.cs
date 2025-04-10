using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Button startButton;
    public Button exitButton;
    public Text roomNameText;
    public Text playerNameText;
    public Image playerChar;

    void Start()
    {
        startButton.onClick.AddListener(OnStartGame);
        exitButton.onClick.AddListener(OnExitRoom);

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
    }

    void Update()
    {
        roomNameText.text = "Sala: " + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void OnExitRoom()
    {
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
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
}