using Assets.Scripts.Interfaces;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using UnityEngine;

public class AINavigator : MonoBehaviour, INavigator
{
    public INeuralNetwork neuralNetwork = null;
    public static double LastFitness = 0;

    internal double x = 0;

    internal float output = 0;
    internal int actualInvidiual;

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
        var inputs = FillInputs();

        output = (float)neuralNetwork.Predict(inputs)[0];

        return output;
    }

    private MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[] FillInputs()
    {
        const double v = 30d;
        int qtdInputs = 32;
        var inputs = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[qtdInputs];

        var sensor1 = GetSensor("Sensor01");
        if(sensor1 != null)
        {
            x = sensor1.transform.position.x / 9f;
            inputs[0] = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(x);

            for (var i = 1; i < qtdInputs; i++)
            {
                var sensorName = i.ToString().PadLeft(2, '0');
                var sensor = GetSensor("Sensor" + sensorName);
                inputs[i] = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(sensor.distanceHit / v);
            }
        }

        return inputs;
    }

    public void Feedback(float points)
    {
        LastFitness = points;

        if(neuralNetwork != null)
            neuralNetwork.Fitness = LastFitness;
    }

    private static SensorController GetSensor(string Name)
    {
        if (GameObject.Find(Name) == null)
            return null;

        return GameObject.Find(Name).GetComponent<SensorController>();
    }

}

