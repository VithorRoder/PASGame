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
    private float rotationSyncTimer;
    private const float RotationSyncRate = 0.05f;

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

        rotationSyncTimer += Time.deltaTime;
    }

    bool TryConsumeSyncTick()
    {
        if (rotationSyncTimer < RotationSyncRate)
            return false;

        rotationSyncTimer = 0f;
        return true;
    }


    void HandleMouseShooting()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePosition - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (lastRotation != rotation && TryConsumeSyncTick())
        {
            lastRotation = rotation;
            photonView.RPC("SyncRotation", RpcTarget.Others, rotZ);
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            FireBullet(rotZ, rotation.normalized);
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

                if (photonView.IsMine && TryConsumeSyncTick())
                {
                    lastRotation = shootDirection;
                    photonView.RPC("SyncRotation", RpcTarget.Others, smoothRotation);
                }

                if (canFire)
                {
                    canFire = false;
                    FireBullet(smoothRotation, shootDirection);
                }
            }
        }
    }

    [PunRPC]
    void SyncRotation(float rotZ)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    void FireBullet(float rotZ, Vector2 shootDirection)
    {
        // PhotonNetwork.Instantiate (instead of a local Instantiate + manual RPC mirror)
        // gives the bullet a real, unique network identity shared by every client,
        // so collisions and destruction are decided consistently instead of each
        // client simulating its own disconnected copy.
        //
        // The initial velocity is passed as instantiation data instead of being
        // set directly on the returned GameObject here: that would only apply on
        // the shooter's own client. Every other client instantiates its own local
        // copy of this networked object through PUN internally (not by running
        // this method), so their Rigidbody2D would otherwise stay at zero
        // velocity — the bullet would spawn but never actually move or reach
        // anyone on their screen. BulletScript.OnPhotonInstantiate reads this
        // data and applies it identically on every client.
        Vector2 bulletVelocity = shootDirection.normalized * force;
        object[] instantiationData = { bulletVelocity };
        PhotonNetwork.Instantiate(
            bulletPrefab.name, bulletTransform.position, Quaternion.Euler(0, 0, rotZ), 0, instantiationData);
    }

    bool IsMobile()
    {
        return Application.isMobilePlatform;
    }
}