using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class BulletScript : MonoBehaviourPun
{
    private Rigidbody2D rb;
    public float damage = 5;
    private bool collided = false;
    public PhotonView photonBullet;
    public GameObject explosionPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        photonBullet = GetComponent<PhotonView>();
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (!collided)
        {
            if (photonBullet != null && photonBullet.IsMine)
            {
                photonBullet.RPC("DestroyBullet", RpcTarget.AllBuffered);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rb.linearVelocity = transform.up;
        }
    }

    [PunRPC]
    void DestroyBullet()
    {
        if (!collided)
        {
            collided = true;
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            HandleCollision();
            return;
        }

        string bulletTag = gameObject.tag;
        string playerTag = other.gameObject.tag;


        if (other.CompareTag("BulletBlue") || other.CompareTag("BulletRed") || other.CompareTag("BulletPink") || other.CompareTag("BulletGreen") || other.CompareTag("BulletYellow") || other.CompareTag("BulletWhite"))
        {
            HandleCollision();
            return;
        }

        switch (bulletTag)
        {
            case "BulletBlue":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletRed":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterBlue", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletPink":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterBlue", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletGreen":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterBlue", "CharacterYellow", "CharacterWhite"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletYellow":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterBlue", "CharacterWhite"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletWhite":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterBlue"))
                {
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            default:
                break;
        }
    }

    bool IsPlayerAllowedToBeHit(string playerTag, params string[] allowedTags)
    {
        return allowedTags.Contains(playerTag);
    }

    [PunRPC]
    void HandleCollision()
    {
        if (!collided)
        {
            collided = true;
            if (explosionPrefab != null)
            {
                PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);
            }
            PhotonView photonView = this.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                photonView.RPC("DestroyBullet", RpcTarget.AllBuffered);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void DamagePlayer(GameObject player)
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();

        if (ph != null && ph.photonView.IsMine)
        {
            ph.ReduceHealth(damage);
        }
    }
}