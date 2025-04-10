using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RefreshButton : MonoBehaviour // Herdar de MonoBehaviour
{
    public void RefreshRoomList() // O método precisa ser público
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby(); // Sai do lobby para garantir que a atualização ocorra
        }
        PhotonNetwork.JoinLobby(); // Reentra no lobby para atualizar a lista de salas
    }
}