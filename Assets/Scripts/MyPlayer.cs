using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class MyPlayer : MonoBehaviourPun, IPunObservable
{

    public PhotonView pv;
    public float moveSpeed = 400;
    public float jumpforce = 600;
    private Vector3 smoothMove;
    private GameObject sceneCamera;
    public Text nameText;
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    private bool IsGrounded;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.07f;

    void Start()
    {
        if (photonView.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;
            rb = GetComponent<Rigidbody2D>();
            sceneCamera = GameObject.Find("Main Camera");

            sceneCamera.SetActive(true);
        }
        else
        {
            nameText.text = pv.Owner.NickName;
        }
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            smoothMovement();
        }
    }

    private void smoothMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, smoothMove, ref velocity, smoothTime);
    }

    private void ProcessInputs()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            pv.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pv.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pv.RPC("OnDirectionChange_UP", RpcTarget.Others);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            pv.RPC("OnDirectionChange_DOWN", RpcTarget.Others);
        }

    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (photonView.IsMine)
        {
            if (col.gameObject.tag == "Ground")
            {
                IsGrounded = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (photonView.IsMine)
        {
            if (col.gameObject.tag == "Ground")
            {
                IsGrounded = false;
            }
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }

    }
}
