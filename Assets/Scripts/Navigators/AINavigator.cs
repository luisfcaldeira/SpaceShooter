using Assets.Scripts.Interfaces;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using UnityEngine;

public class AINavigator : MonoBehaviour, INavigator
{
    public INeuralNetwork neuralNetwork = null;
    public static double LastFitness = 0;

    internal double input1 = 0;
    internal double input2 = 0;
    internal double input3 = 0;
    internal double input4 = 0;
    internal double input5 = 0;
    internal double x = 0;

    internal float output = 0;

    public Vector2 Move()
    {
        

        float movement = 0;

        if (neuralNetwork != null)
        {
            movement = GenerateMovement();
        }
        return new Vector2(movement, 0);
    }

    private float GenerateMovement()
    {
        var rightSensor = GetSensor("RightSensor");
        var leftSensor = GetSensor("LeftSensor");
        var sensor04 = GetSensor("Sensor04");
        var sensor05 = GetSensor("Sensor05");

        var frontSensor = GetFrontSensor();

        const double v = 30d;

        input1 = frontSensor.distanceHit / v;
        input2 = rightSensor.distanceHit / v;
        input3 = leftSensor.distanceHit / v;
        input4 = sensor04.distanceHit / v;
        input5 = sensor05.distanceHit / v;

        if (input1 == 0 && input2 == 0 && input3 == 0 && input4 == 0 && input5 == 0)
            return 0;

        if(rightSensor.distanceHit <= 1 || leftSensor.distanceHit <= 1)
        {
            GetComponent<PlayerController>().Die();
        }

        x = GetSensor("Sensor01").transform.position.x / v;

        output = (float)neuralNetwork.Predict(
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[] {
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(x),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input1),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input2),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input3),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input4),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input5),
                    })[0];


        return output;
    }

    private static SensorController GetFrontSensor()
    {
        var sensor01 = GetSensor("Sensor01");
        var sensor02 = GetSensor("Sensor02");
        var sensor03 = GetSensor("Sensor03");

        var result = sensor01;

        if (sensor02.distanceHit < sensor01.distanceHit)
            result = sensor02;

        if(sensor03.distanceHit < sensor02.distanceHit) 
            result = sensor03;

        return result;
    }

    public void Feedback(float points)
    {
        LastFitness = points;

        if(neuralNetwork != null)
            neuralNetwork.Fitness = LastFitness;
    }

    private static SensorController GetSensor(string Name)
    {
        return GameObject.Find(Name).GetComponent<SensorController>();
    }

}

