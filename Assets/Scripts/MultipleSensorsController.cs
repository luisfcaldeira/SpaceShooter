using Assets.Scripts.Entities.Sensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSensorsController : MonoBehaviour
{
    [SerializeField]
    private float numberOfSensors = 100;
    private float degrees = 360; 

    [SerializeField]
    private LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var distance = 10f;
        var actualDegree = 0f;
        var degreeIncrement = DegreeToRadians(degrees) / numberOfSensors;
        var radian = DegreeToRadians(actualDegree);

        for(int i = 0; i < numberOfSensors;  i++)
        {
            var ray = RayBuilder.WithOrigin(transform.position)
                .WithHorizontalDirection(distance * Mathf.Sin(radian))
                .WithVerticalDirection(distance * Mathf.Cos(radian))
                .Build();

            Sensor.WithDefaultDistance()
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
