public class EnemyController : BaseEnemyShip, IHit
{
    

    void Update()
    {
        Move();
        
        if(mustShoot)
            Shoot();

        CheckLifeAndDie();
    }

}
