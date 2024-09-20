using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject deathPanel;

    float health;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / maxHealth;
        if (health <= 0)
        {
            gameObject.GetComponent<PlayerStateManager>().isAlive = false;
            Death();
        }
    }
    void Death()
    {
        deathPanel.SetActive(true);
    }
}
