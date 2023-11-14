using UnityEngine;

public class ShieldController : MonoBehaviour
{
    [SerializeField]
    private float life = 5f;

    internal GameObject player;

    void Update()
    {
        Move();
        DieIfLifeIsOver();
    }

    private void Move()
    {
        Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y);
        transform.position = newPosition;
    }

    private void DieIfLifeIsOver()
    {
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if(collision.CompareTag("Tiro Enemy") || collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            life--;
        }
    }
}
