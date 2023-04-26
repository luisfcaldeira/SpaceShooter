using Assets.Scripts.Entities.Sensors;
using UnityEngine;

public class MultipleSensorsController : MonoBehaviour
{
    internal int numberOfSensors = 40;
    private float degrees = 360; 

    [SerializeField]
    private LayerMask layerMask;

    internal RaycastHit2D[] hits;

    // Start is called before the first frame update
    void Start()
    {
        hits = new RaycastHit2D[numberOfSensors];
    }

    // Update is called once per frame
    void Update()
    {
        var distance = 10f;
        var actualDegree = 0f;
        var degreeIncrement = DegreeToRadians(degrees) / (float)numberOfSensors;
        var radian = DegreeToRadians(actualDegree);

        for(int i = 0; i < numberOfSensors;  i++)
        {
            var ray = RayBuilder.WithOrigin(transform.position)
                .WithHorizontalDirection(distance * Mathf.Sin(radian))
                .WithVerticalDirection(distance * Mathf.Cos(radian))
                .Build();

            hits[i] = Sensor.WithDefaultDistance()
                .Draw(ray)
                .WithLayerMask(layerMask)
                .GetHit(ray);
             
            radian += degreeIncrement;
        }
    }

    private float DegreeToRadians(float degree)
    {
        return degree * Mathf.Deg2Rad;
    }
}
