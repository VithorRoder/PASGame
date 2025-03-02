using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.InputSystem;

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
    private PlayerControls controls;
    private Vector2 moveInput;


    void Start()
    {
        if (photonView.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;
            rb = GetComponent<Rigidbody2D>();
            sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(true);

            controls = new PlayerControls();
            controls.Enable();

            if (Application.isMobilePlatform)
            {
                controls.PlayerMovement.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
                controls.PlayerMovement.Movement.canceled += ctx => moveInput = Vector2.zero;
            }
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
        if (Application.isMobilePlatform)
        {
            Vector2 move = moveInput;
            smoothMove = new Vector3(move.x, move.y, 0) * moveSpeed * Time.deltaTime;
        }
        else
        {
            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            smoothMove = new Vector3(move.x, move.y, 0) * moveSpeed * Time.deltaTime;

            if (move.x > 0)
            {
                pv.RPC("OnDirectionChange_RIGHT", RpcTarget.Others);
            }
            else if (move.x < 0)
            {
                pv.RPC("OnDirectionChange_LEFT", RpcTarget.Others);
            }

            if (move.y > 0)
            {
                pv.RPC("OnDirectionChange_UP", RpcTarget.Others);
            }
            else if (move.y < 0)
            {
                pv.RPC("OnDirectionChange_DOWN", RpcTarget.Others);
            }
        }
        transform.position += smoothMove;
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