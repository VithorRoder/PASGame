using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class BulletScript : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public float damage = 5;
    private bool collided = false;
    public PhotonView photonBullet;
    public GameObject explosionPrefab;
    private Rigidbody2D rb;
    private bool processed = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        photonBullet = GetComponent<PhotonView>();
        StartCoroutine(DestroyAfterDelay());
    }

    // Called by PUN on every client (shooter included) right after the bullet
    // is network-instantiated. The initial velocity travels as instantiation
    // data because setting it directly on the GameObject returned by
    // PhotonNetwork.Instantiate only ever affects the shooter's own local copy.
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0 && data[0] is Vector2 velocity)
        {
            rb.linearVelocity = velocity;
        }
    }

    void FixedUpdate()
    {
        // Trigger overlaps in Unity's 2D physics aren't swept between steps, so a
        // fast bullet can fully cross a thin wall (or a player) within a single
        // FixedUpdate without ever overlapping its collider, and OnTriggerEnter2D
        // never fires. Cast forward along this step's velocity BEFORE physics
        // moves the object, so the crossing is caught instead of missed.
        // Runs on every client (like OnTriggerEnter2D already does) since
        // ownership checks for what to actually do live inside HandleCollision
        // (bullet owner) and DamagePlayer (hit player's own owner).
        if (!collided)
        {
            Vector2 velocity = rb.linearVelocity;
            float step = velocity.magnitude * Time.fixedDeltaTime;

            if (step > 0f)
            {
                // RaycastAll (not Raycast) because the ray starts at the bullet's
                // own center: a plain Raycast would often return the bullet's own
                // collider as the closest hit, hiding whatever is further along.
                // Walk every hit in order (closest first) instead of stopping at
                // the first non-self one: a hit that results in no action (e.g.
                // a same-color teammate, which is meant to be passed through)
                // must not hide a wall standing right behind it in the same step.
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, velocity, step);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider == null || hit.collider.gameObject == gameObject)
                        continue;

                    ProcessCollision(hit.collider);
                    if (processed)
                        break;
                }
            }
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        if (!collided && photonView.IsMine)
        {
            // Only the owner may call PhotonNetwork.Destroy (see HandleCollision
            // for why) — Photon propagates the removal to every other client's
            // copy automatically once the owner calls it.
            collided = true;
            photonView.RPC("FreezeBullet", RpcTarget.Others);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void FreezeBullet()
    {
        // Visual-only: stops the bullet and hides it on every OTHER client
        // right away, without waiting for PhotonNetwork's own (slightly
        // slower) destroy-propagation to actually remove the GameObject.
        collided = true;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ProcessCollision(other);
    }

    void ProcessCollision(Collider2D other)
    {
        // A bullet only ever handles its first *real* collision. This guard
        // must only be set once we've actually decided to act (wall, bullet
        // vs bullet, or a valid enemy hit) — not on every call — otherwise an
        // irrelevant early overlap (e.g. the bullet spawning inside its own
        // shooter, same color, falls through to the default/no-op case) would
        // burn the bullet's one shot and silently swallow the real hit that
        // comes right after, on the same bullet, moments later.
        if (processed)
            return;

        if (other.CompareTag("Ground"))
        {
            processed = true;
            HandleCollision();
            return;
        }

        string bulletTag = gameObject.tag;
        string playerTag = other.gameObject.tag;


        if (other.CompareTag("BulletBlue") || other.CompareTag("BulletRed") || other.CompareTag("BulletPink") || other.CompareTag("BulletGreen") || other.CompareTag("BulletYellow") || other.CompareTag("BulletWhite"))
        {
            processed = true;
            HandleCollision();
            return;
        }

        switch (bulletTag)
        {
            case "BulletBlue":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    processed = true;
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletRed":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterBlue", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    processed = true;
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletPink":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterBlue", "CharacterGreen", "CharacterYellow", "CharacterWhite"))
                {
                    processed = true;
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletGreen":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterBlue", "CharacterYellow", "CharacterWhite"))
                {
                    processed = true;
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletYellow":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterBlue", "CharacterWhite"))
                {
                    processed = true;
                    HandleCollision();
                    DamagePlayer(other.gameObject);
                }
                break;

            case "BulletWhite":
                if (IsPlayerAllowedToBeHit(playerTag, "CharacterRed", "CharacterPink", "CharacterGreen", "CharacterYellow", "CharacterBlue"))
                {
                    processed = true;
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

    void HandleCollision()
    {
        // Only the bullet's owner decides that a collision happened and tells
        // everyone else via RPC. Without this gate, every client that detects
        // the same collision locally would spawn its own explosion and try to
        // destroy the bullet, causing duplicate explosions and RPC race conditions.
        if (collided || !photonView.IsMine)
            return;

        collided = true;

        // Stop the bullet locally right away instead of waiting for the
        // DestroyBullet RPC round-trip: PUN RPCs aren't synchronous even for
        // the caller, so without this the bullet keeps physically moving
        // (and can sail well past a wall) during that gap before it's
        // actually destroyed.
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        if (explosionPrefab != null)
        {
            PhotonNetwork.Instantiate(explosionPrefab.name, transform.position, Quaternion.identity);
        }

        // Only the owner is allowed to call PhotonNetwork.Destroy — Photon
        // rejects the call from anyone else (that's the "neither owner nor
        // MasterClient" error), so a DestroyBullet-for-everyone RPC used to
        // silently fail (and leak the GameObject) on every other client.
        // FreezeBullet (visual-only, no ownership requirement) keeps the
        // other clients' copies looking dead in the meantime.
        photonView.RPC("FreezeBullet", RpcTarget.Others);
        PhotonNetwork.Destroy(gameObject);
    }

    void DamagePlayer(GameObject player)
    {
        // Only the bullet's owner reports the hit. ProcessCollision runs on
        // every client's own local copy of this same networked bullet, so
        // without this gate both the shooter's client AND the victim's own
        // client can independently detect the same collision and each send
        // a damage RPC — applying the hit twice.
        if (!photonView.IsMine)
            return;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph == null)
            return;

        // Tell the hit player's own owner to apply the damage, instead of
        // relying on their client independently re-detecting this same
        // collision through their own local physics simulation. The bullet
        // owner's detection here is the reliable one (it's local to them,
        // zero network latency); the victim's own copy of this bullet can
        // legitimately never get a chance to overlap it before the network
        // destroy event removes the object, especially with a bullet this
        // fast — which was silently dropping hits on the victim's side.
        ph.photonView.RPC("ReduceHealth", ph.photonView.Owner, damage);
    }
}