using Assets.Scripts.Interfaces;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHit
{
    
    [SerializeField]
    private float velocity = 5f;

    [SerializeField]
    internal int life = 100;

    [SerializeField]
    private float fireCooldown = 0.3f;

    private float actualFireCooldown = 0;

    private Rigidbody2D myRigidbody;

    [SerializeField]
    private GameObject bullet1;
    
    [SerializeField]
    private GameObject bullet2;

    [SerializeField]
    private Transform bulletExit;

    [SerializeField]
    private GameObject shipExplosion;

    [SerializeField]
    protected float limitOfX, limitOfY;

    [SerializeField]
    protected float levelWeapon = 1;

    [SerializeField]
    private GameObject shield;

    [SerializeField]
    private float amountShieldCooldown = 30f;

    [SerializeField]
    private float shieldCooldown;

    private GameObject instanceOfShield;

    [SerializeField]
    public INavigator navigator;

    [SerializeField]
    private float lifeExpectancy = 60 * 10;

    internal Vector2 myPosition { get; private set; }

    public bool IsAlive { get; private set; } = true;

    public float ReturnToZeroPoints { get; private set; }
    public float DistancePoints { get; private set; }
    public float MeanX { get; private set; }
    public float LifePoints { get; private set; }
    public float DeadEnemies { get; private set; }
    public float Points { get; private set; }

    protected float traveledDistance = 0;
    protected float distanceToTravel = 10000f;

    private float elapsedTimeOfLife = 0;

    private float returnToZero = 0;
    private float returnToZeroCounter = 0;
    private float maxX = 0;
    private float minX = 0;
    private float horDiff = 0.01f;

    internal float deadEnemies = 0;

    [SerializeField]
    private float avoidEnemies = 100f;

    void Start()
    {
        navigator = GetComponent<INavigator>();
        shieldCooldown = 0;
        actualFireCooldown = fireCooldown;
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();

        FeedbackNavigator();

        Fire();

        FixPosition();

        PosComb();

        CheckLifeAndDie();

        GenerateShield();
    }

    private void Move()
    {
        var newVelocity = navigator.Move();

        if (newVelocity.x > 0 && transform.position.x < limitOfX
            || newVelocity.x < 0 && transform.position.x > -limitOfX)
        {
            traveledDistance += Mathf.Abs(newVelocity.x);
            myRigidbody.velocity = newVelocity * velocity;
        }
    }

    private void FeedbackNavigator()
    {
        elapsedTimeOfLife += Time.deltaTime;

        if (transform.position.x < 0)
        {
            maxX = 0;
            if (transform.position.x < minX)
            {
                minX = transform.position.x;
            }

            if (transform.position.x > minX + horDiff)
            {
                returnToZero += Mathf.Abs(transform.position.x - minX) / 9f;
                minX = transform.position.x;
                returnToZeroCounter++;
                horDiff *= Mathf.Min(horDiff * 2, 1f);
            }
        }

        if (transform.position.x > 0)
        {
            minX = 0;
            if (transform.position.x > maxX)
            {
                maxX = transform.position.x;
            }

            if (transform.position.x < maxX - horDiff)
            {
                returnToZero += (maxX - transform.position.x) / 9f;
                maxX = transform.position.x;
                returnToZeroCounter++;
                horDiff = Mathf.Min(horDiff * 2, 1f);
            }
        }

        ReturnToZeroPoints = 0;
        if (returnToZeroCounter != 0)
            ReturnToZeroPoints = returnToZero * 2 / returnToZeroCounter;

        DistancePoints = traveledDistance / distanceToTravel;
        LifePoints = elapsedTimeOfLife / lifeExpectancy;
        DeadEnemies = deadEnemies * 4f / avoidEnemies;

        Points = (LifePoints + ReturnToZeroPoints + DeadEnemies) / (2 + 4 + 1);

        navigator.Feedback(Points);
    }

    private void FixPosition()
    {
        var actualX = Mathf.Clamp(transform.position.x, -limitOfX, limitOfX);
        var actualY = Mathf.Clamp(transform.position.y, -limitOfY, limitOfY);

        Vector3 newPosition = new Vector3(actualX, actualY);
        transform.position = new Vector2(newPosition.x, newPosition.y);
        myPosition = new Vector2(newPosition.x, newPosition.y);
    }

    public void Fire()
    {
        actualFireCooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1") && actualFireCooldown <= 0)
        {
            switch (levelWeapon)
            {
                case 1:
                    InstantiateBulletAndFire(bullet1, bulletExit.position);
                    break;
                case 2:
                    InstantiateBulletAndFire(bullet2, bulletExit.position + new Vector3(0.5f, -0.2f, 0));
                    InstantiateBulletAndFire(bullet2, bulletExit.position + new Vector3(-0.5f, -0.2f, 0));
                    break;
                case 3:
                    InstantiateBulletAndFire(bullet1, bulletExit.position);
                    InstantiateBulletAndFire(bullet2, bulletExit.position + new Vector3(0.5f, -0.2f, 0));
                    InstantiateBulletAndFire(bullet2, bulletExit.position + new Vector3(-0.5f, -0.2f, 0));
                    break;
                default:
                    InstantiateBulletAndFire(bullet1, bulletExit.position);
                    break;
            }

            actualFireCooldown = fireCooldown;
        }
    }

    private void InstantiateBulletAndFire(GameObject bullet, Vector3 newPosition)
    {
        var instance = Instantiate(bullet, newPosition, transform.rotation);
        instance.GetComponent<ShotController>().Fire(Vector2.up);
    }

    private void PosComb()
    {
        var animator = GetComponent<Animator>();

        animator.SetBool("TurboOn", Input.GetAxis("Vertical") > 0);
        animator.SetBool("EngineOff", Input.GetAxis("Vertical") < 0);
    }


    private void CheckLifeAndDie()
    {
        if(life <= 0)
        {
            Die();
        }
    }

    internal void Die()
    {
        Destroy(gameObject);
        Instantiate(shipExplosion, transform.position, transform.rotation);
        IsAlive = false;
    }

    private void GenerateShield()
    {
        if(!IsShieldActivated())
            shieldCooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Fire2") && shieldCooldown <= 0)
        {
            instanceOfShield = Instantiate(shield);
            Destroy(instanceOfShield, 5f);
            var shieldController = instanceOfShield.GetComponent<ShieldController>();
            shieldController.player = gameObject;
            shieldCooldown = amountShieldCooldown;
        }
    }

    private bool IsShieldActivated()
    {
        return instanceOfShield != null;
    }

    public void Hit(int damage)
    {
        life -= damage;
    }

    public void LevelUpWeapon()
    {
        if(levelWeapon < 3)
        {
            levelWeapon++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Destruidor"))
        {
            //Die();
        }
    }
}
