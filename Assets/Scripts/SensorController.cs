using UnityEngine;


public class SensorController : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;

    public float distanceHit = 0;

    [SerializeField]
    private float verticalDirection = 0;

    [SerializeField]
    private float horizontalDirection = 0;

    Ray ray;

    void Start()
    {
    }

    void Update()
    {
        distanceHit = 0;

        Vector2 direction = new Vector2(verticalDirection, horizontalDirection);

        ray = new Ray(transform.position, direction);

        Debug.DrawRay(ray.origin, direction);

        var hit = Physics2D.Raycast(ray.origin, direction, 50, enemyLayer);

        if (hit.collider)
        {
            distanceHit = hit.distance;
        }
    }
}
