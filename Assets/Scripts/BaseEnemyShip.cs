using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class BaseEnemyShip : MonoBehaviour
{
    protected Rigidbody2D myRigidbody;

    public Action ActionOnDestroy { get; set; }

    [SerializeField]
    internal float velocity = 0.5f;

    [SerializeField]
    protected float timeToFire = 1f;

    protected float countTimeToFire;

    [SerializeField]
    protected GameObject shipExplosion;

    [SerializeField]
    protected int life = 5;

    protected SpriteRenderer childSpriteRenderer;

    [SerializeField]
    protected GameObject shot;    
    
    [SerializeField]
    protected GameObject shot2;

    [SerializeField]
    internal bool mustShoot = true;

    [SerializeField]
    protected int damageOnCollide = 5;


    [SerializeField]
    private Transform bulletExit;

    [SerializeField]
    private bool straigthMove = true;

    [SerializeField]
    protected int xpPoints = 1;

    protected float maxX, maxY, minX, minY;

    [SerializeField]
    private float chanceOfDropBoost = 0.05f;

    [SerializeField]
    protected GameObject powerUp;

    void Start()
    {
        maxX = 5;
        minX = -5;
        maxY = 8.9f;
        minY = -8.9f;

        myRigidbody = GetComponent<Rigidbody2D>();

        countTimeToFire = timeToFire;

        childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        DropPowerUp();
    }

    protected void Move()
    {
        float vertical = 0;
        if (!straigthMove)
        {
            vertical = Mathf.Sin(transform.position.y) * 3;
        }

        myRigidbody.velocity = new Vector2(vertical, -velocity); 
    }

    protected void Shoot()
    {
        countTimeToFire -= Time.deltaTime;
        var playerController = FindObjectOfType<PlayerController>();

        if (mustShoot && (countTimeToFire <= 0 && childSpriteRenderer.isVisible && playerController))
        {
            countTimeToFire = timeToFire;
            var shotInstance = Instantiate(shot, bulletExit);
            var shotController = shotInstance.GetComponent<ShotController>();

            var direction = shotInstance.transform.position - playerController.transform.position;
            direction.Normalize();

            shotController.Fire(direction);
        }
    }

    public void Hit(int damage)
    {
        if(childSpriteRenderer.isVisible)
        {
            if(transform.position.y < maxY)
                life -= damage;
            if(life <= 0)
            {
                FindObjectOfType<GameController>().UpXp(xpPoints);
                DropPowerUp();
            }
        }
    }

    protected void CheckLifeAndDie()
    {
        if (life <= 0)
        {
            Die();
        }
    }

    internal void Die()
    {
        Destroy(gameObject);
    }

    private void DropPowerUp()
    {
        var result = Random.Range(0f, 1f);
        if (result < chanceOfDropBoost)
        {
            Instantiate(powerUp, transform.position, transform.rotation);
        }
    }

    private void OnDestroy()
    {
        Instantiate(shipExplosion, transform.position, transform.rotation);
        ActionOnDestroy?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Destruidor"))
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<IHit>()?.Hit(damageOnCollide);
            Die();
        }
    }
}

