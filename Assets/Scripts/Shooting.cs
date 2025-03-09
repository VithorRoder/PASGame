using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    private Camera mainCamera;
    private Vector3 lastRotation;
    public GameObject bulletPrefab;
    public Transform bulletTransform;
    public bool canFire;
    private float timer;
    public float timeBFire;
    public float force;
    public PhotonView pv;
    public RightStickController rightStick;
    private float currentVelocity;

    void Start()
    {
        mainCamera = Camera.main;

        GameObject rightStickObject = GameObject.FindGameObjectWithTag("RightStick");
        if (rightStickObject != null)
        {
            rightStick = rightStickObject.GetComponent<RightStickController>();
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (!IsMobile())
        {
            HandleMouseShooting();
        }
        else
        {
            HandleRightStickShooting();
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
    }

    void HandleMouseShooting()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (lastRotation != rotation)
        {
            lastRotation = rotation;
            photonView.RPC("SyncRotation", RpcTarget.All, rotZ);
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            ShootBullet(rotZ, mousePosition, rotation.normalized);
        }
    }

    void HandleRightStickShooting()
    {
        if (rightStick.IsTouching())
        {
            Vector2 shootDirection = rightStick.stickDirection.normalized;

            if (shootDirection.sqrMagnitude > 0.01f)
            {
                float targetRotZ = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
                float smoothRotation = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, targetRotZ, ref currentVelocity, 0.1f);
                transform.rotation = Quaternion.Euler(0, 0, smoothRotation);

                if (Vector3.Distance(lastRotation, shootDirection) > 0.15f)
                {
                    lastRotation = shootDirection;
                    photonView.RPC("SyncRotation", RpcTarget.All, smoothRotation);
                }

                Vector3 aimPosition = bulletTransform.position + (Vector3)shootDirection * 1.2f;

                if (canFire)
                {
                    canFire = false;
                    ShootBullet(smoothRotation, aimPosition, shootDirection);
                }
            }
        }
    }

    [PunRPC]
    void SyncRotation(float rotZ)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    [PunRPC]
    void ShootBullet(float rotZ, Vector3 targetPosition, Vector2 shootDirection)
    {
        GameObject newBullet = Instantiate(bulletPrefab, bulletTransform.position, Quaternion.Euler(0, 0, rotZ));
        newBullet.GetComponent<Rigidbody2D>().velocity = shootDirection * force;

        photonView.RPC("NetworkShootBullet", RpcTarget.Others, newBullet.transform.position, shootDirection * force);
    }

    [PunRPC]
    void NetworkShootBullet(Vector3 position, Vector2 velocity)
    {
        GameObject newBullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        newBullet.GetComponent<Rigidbody2D>().velocity = velocity;
    }

    bool IsMobile()
    {
        return Application.isMobilePlatform;
    }
}