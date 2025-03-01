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

    [PunRPC]
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        health = currentHealth;
        maxHealth = maxHealth;
        healthBall.fillAmount = Mathf.Clamp(GetCurrentHealth() / GetMaxHealth(), 0, 1);
    }

    public void Initialize(float initialHealth)
    {
        maxHealth = initialHealth;
        health = maxHealth;
    }

    [PunRPC]
    public void ReduceHealth(float amount)
    {
        if (!photonView.IsMine) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!photonView.IsMine) return;

        PhotonNetwork.Destroy(gameObject);

        PhotonNetwork.LoadLevel(2);
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

}
