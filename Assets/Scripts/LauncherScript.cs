using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
  public GameObject ConnectedScreenS;
  public GameObject ConnectedScreenF;
  public GameObject Connecting;
  public GameObject TitleLobby;
  public GameObject SuccessfullyConnected;

  void Awake()
  {
  }

  public void OnClick_ConnectBt()
  {
    Connecting.SetActive(true);
    PhotonNetwork.ConnectUsingSettings();
  }

  public override void OnConnectedToMaster()
  {
    PhotonNetwork.JoinLobby(TypedLobby.Default);
  }

  public override void OnDisconnected(DisconnectCause cause)
  {
    ConnectedScreenF.SetActive(true);
  }

  public override void OnJoinedLobby()
  {
    if (ConnectedScreenF.activeSelf)
    {
      ConnectedScreenF.SetActive(false);
    }

    ConnectedScreenS.SetActive(true);
    TitleLobby.SetActive(true);
    StartCoroutine(HideAfterTime(5f));
  }
  IEnumerator HideAfterTime(float seconds)
  {
    yield return new WaitForSeconds(seconds);
    SuccessfullyConnected.SetActive(false);
  }
}
