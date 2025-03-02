using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    public Image healthBall;
    public float health;
    public float maxHealth;
    public PhotonView pvv;

    void Start()
    {
        Initialize(100f);
    }

    public void Initialize(float initialHealth)
    {
        maxHealth = initialHealth;
        health = maxHealth;
        UpdateHealthUI();
    }

    [PunRPC]
    public void ReduceHealth(float amount)
    {
        if (!photonView.IsMine) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        photonView.RPC("UpdateHealth", RpcTarget.All, health);

        if (health <= 0)
        {
            photonView.RPC("Die", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void UpdateHealth(float newHealth)
    {
        health = newHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthBall.fillAmount = Mathf.Clamp(health / maxHealth, 0, 1);
    }

    [PunRPC]
    private void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.LoadLevel(2);
        }
    }
}