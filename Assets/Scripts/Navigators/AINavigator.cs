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
        float movePredict;
        var rightSensor = GameObject.Find("RightSensor").GetComponent<SensorController>();
        var leftSensor = GameObject.Find("LeftSensor").GetComponent<SensorController>();

        var frontSensor = GetFrontSensor();

        float enemyVerticalDiff = EnemyVerticalDiff();

        double input1 = frontSensor / 10d;
        double input2 = rightSensor.distanceHit / 10d;
        double input3 = leftSensor.distanceHit / 10d;

        movePredict = (float)neuralNetwork.Predict(
            new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[] {
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(enemyVerticalDiff),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input1),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input2),
                    new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(input3),
            })[0];
        return movePredict;
    }

    private static float EnemyVerticalDiff()
    {
        var sensor01 = GameObject.Find("Sensor01").GetComponent<SensorController>();
        float enemyX = EnemyHitPenalty();
        return Mathf.Abs(enemyX - sensor01.gameObject.transform.position.x) / 10f;
    }

    private static float GetFrontSensor()
    {
        var sensor01 = GameObject.Find("Sensor01").GetComponent<SensorController>();
        var sensor02 = GameObject.Find("Sensor02").GetComponent<SensorController>();
        var sensor03 = GameObject.Find("Sensor02").GetComponent<SensorController>();

        return Mathf.Min(sensor01.distanceHit, sensor02.distanceHit, sensor03.distanceHit);
    }

    private static float EnemyHitPenalty()
    {
        var sensor01 = GameObject.Find("Sensor01").GetComponent<SensorController>();
        var sensor02 = GameObject.Find("Sensor02").GetComponent<SensorController>();
        var sensor03 = GameObject.Find("Sensor02").GetComponent<SensorController>();

        var enemyX = sensor01.gameObject.transform.position.x;

        if (sensor02.distanceHit < sensor01.distanceHit)
        {
            enemyX = sensor02.gameObject.transform.position.x;
        }

        if (sensor03.distanceHit < sensor02.distanceHit)
        {
            enemyX = sensor03.gameObject.transform.position.x;
        }

        return enemyX;
    }

    public void ElapsedTime(float elapsedTime)
    {
        var rightSensor = GameObject.Find("RightSensor").GetComponent<SensorController>();
        var leftSensor = GameObject.Find("LeftSensor").GetComponent<SensorController>();
        
        var enemyHitPenalty = Mathf.Pow(EnemyVerticalDiff(), 2);

        LastFitness = elapsedTime
            + Mathf.Abs(leftSensor.distanceHit - rightSensor.distanceHit) / 10d
            + enemyHitPenalty;

        neuralNetwork.Fitness = LastFitness;
    }

}

