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

    public bool IsAlive { get; private set; } = true;
    float elapsedTimeOfLife = 0;

    [SerializeField]
    private float lifeExpectancy = 60 * 10;

    void Start()
    {
        navigator = GetComponent<INavigator>();
        shieldCooldown = 0;
        actualFireCooldown = fireCooldown;
        myRigidbody = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        elapsedTimeOfLife += Time.deltaTime;
        navigator.ElapsedTime((lifeExpectancy - elapsedTimeOfLife) / lifeExpectancy);

        var newVelocity = navigator.Move();

        myRigidbody.velocity = newVelocity * velocity;

        Fire();

        FixPosition();

        PosComb();

        CheckLifeAndDie();

        GenerateShield();
    }

    private void FixPosition()
    {
        var actualX = Mathf.Clamp(transform.position.x, -limitOfX, limitOfX);
        var actualY = Mathf.Clamp(transform.position.y, -limitOfY, limitOfY);

        Vector3 newPosition = new Vector3(actualX, actualY);
        transform.position = newPosition;
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

    private void Die()
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
}
