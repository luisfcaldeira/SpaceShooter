using Assets.Scripts.Entities.Sensors;
using UnityEngine;


public class SensorController : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;

    internal float enemyX = 0;

    public float distanceHit = 0;

    [SerializeField]
    private float verticalDirection = 0;

    [SerializeField]
    private float horizontalDirection = 0;

    void Start()
    {
        
    }

    void Update()
    {
        distanceHit = 0;

        var myRay = RayBuilder.WithOrigin(transform.position)
            .WithHorizontalDirection(horizontalDirection)
            .WithVerticalDirection(verticalDirection)
            .Build();

        var hit = Sensor.WithDefaultDistance()
            .WithLayerMask(enemyLayer)
            .GetHit(myRay);

        if (hit.collider)
        {
            distanceHit = hit.distance;
            if (hit.collider.name.Contains("Enemy"))
            {
                enemyX = hit.collider.transform.position.x;
            }
        }
    }

}
