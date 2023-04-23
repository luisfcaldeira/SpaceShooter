using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if(collision != null && collision.name.Contains("Enem")) { 
            var player = GameObject.Find("GameController").GetComponent<GameController>().instantiatedPlayer;
            var playerController = player.GetComponent<PlayerController>();
            playerController.deadEnemies++;
        }
    }

}
