using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class Shooting : MonoBehaviourPunCallbacks
{
    private Camera mainCamera;
    private Vector3 mousePosition;
    private Vector3 lastRotation;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBFire;
    public float force;
    public PhotonView pv;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (lastRotation != rotation)
        {
            lastRotation = rotation;
            photonView.RPC("SyncRotation", RpcTarget.All, rotZ);
        }

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBFire)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            ShootBullet(rotZ, mousePosition);
        }
    }

    [PunRPC]
    void SyncRotation(float rotZ)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    [PunRPC]
    void ShootBullet(float rotZ, Vector3 mousePos)
    {
        Vector3 direction = mousePos - bulletTransform.position;
        GameObject newBullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.Euler(0, 0, rotZ));
        newBullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * force;

        photonView.RPC("NetworkShootBullet", RpcTarget.Others, newBullet.transform.position, newBullet.GetComponent<Rigidbody2D>().velocity);
    }

    [PunRPC]
    void NetworkShootBullet(Vector3 position, Vector2 velocity)
    {
        GameObject newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().velocity = velocity;
    }
}