using System;
using UnityEngine;

public class ShotController : MonoBehaviour
{

    [SerializeField]
    private float projectileVelocity = 10f;

    [SerializeField]
    private int powerOfFire = 1;

    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private bool followPlayer = false;

    void Start()
    {
    }

    void Update()
    {
    }

    public void Fire(Vector3 direction)
    {
        var myRb = GetComponent<Rigidbody2D>();
        myRb.velocity = Vector2.up * projectileVelocity;

        if (followPlayer)
        {
            myRb.velocity = direction * projectileVelocity;
        }

        PointToPlayer(direction);
    }

    private void PointToPlayer(Vector3 direction)
    {
        var angulo = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        //myRb.rotation = myRb.rotation - angulo;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angulo));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            other.GetComponent<IHit>()?.Hit(powerOfFire);
            
            Instantiate(explosion, transform.position, transform.rotation);
            
            Destroy(gameObject);
        }
    }
}
