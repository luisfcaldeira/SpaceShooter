using Assets.Scripts.Interfaces;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using UnityEngine;

public class AINavigator : MonoBehaviour, INavigator
{
    public INeuralNetwork neuralNetwork = null;
    public static double LastFitness = 0;

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

        var frontSensor = GetFrontSensor();

        double input1 = frontSensor / 10d;
        double input2 = rightSensor.distanceHit / 10d;
        double input3 = leftSensor.distanceHit / 10d;

        return (float)neuralNetwork.Predict(
            new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[] {
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(EnemyHorizontalDiff()),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input1),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input2),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input3),
            })[0];
    }

    private static float EnemyHorizontalDiff()
    {
        var sensor01 = GetSensor("Sensor01");
        float enemyX = EnemyHitPenalty();
        return Mathf.Max(1 - Mathf.Abs(enemyX - sensor01.gameObject.transform.position.x), 0);
    }

    private static float GetFrontSensor()
    {
        var sensor01 = GetSensor("Sensor01");
        var sensor02 = GetSensor("Sensor02");
        var sensor03 = GetSensor("Sensor03");

        return Mathf.Min(sensor01.distanceHit, sensor02.distanceHit, sensor03.distanceHit);
    }

    private static float EnemyHitPenalty()
    {
        var sensor01 = GetSensor("Sensor01");
        var sensor02 = GetSensor("Sensor02");
        var sensor03 = GetSensor("Sensor03");

        var enemyX = sensor01.enemyX;
        var minDistance = sensor01.distanceHit;

        if (sensor02.distanceHit != 0 && (sensor02.distanceHit < minDistance || minDistance == 0))
        {
            enemyX = sensor02.enemyX;
            minDistance = sensor02.distanceHit;
        }

        if (sensor03.distanceHit != 0 && (sensor03.distanceHit < minDistance || minDistance == 0))
        {
            enemyX = sensor03.enemyX;
        }

        return enemyX;
    }

    public void Feedback(float points)
    {
        var rightSensor = GetSensor("RightSensor");
        var leftSensor = GetSensor("LeftSensor");

        var enemyHitPenalty = EnemyHorizontalDiff();

        LastFitness = points + enemyHitPenalty + Mathf.Max(2 - rightSensor.distanceHit, 0) + Mathf.Max(2 - leftSensor.distanceHit, 0);

        neuralNetwork.Fitness = LastFitness;
    }

    private static float GetDistanceWall(SensorController rightSensor, SensorController leftSensor)
    {
        return Mathf.Abs((leftSensor.distanceHit - rightSensor.distanceHit) / 10f);
    }

    private static SensorController GetSensor(string Name)
    {
        return GameObject.Find(Name).GetComponent<SensorController>();
    }

}

