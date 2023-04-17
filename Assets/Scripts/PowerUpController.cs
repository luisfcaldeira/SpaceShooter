using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private float timeOfLife = 2f;

    void Start()
    {
        
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if(timeOfLife <= 0)
        {
            Die();
        }

        timeOfLife -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerController.LevelUpWeapon();
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("power up");
        Destroy(gameObject);
    }
}
