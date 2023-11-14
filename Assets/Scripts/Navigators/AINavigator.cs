using Assets.Scripts.Interfaces;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using UnityEngine;

public class AINavigator : MonoBehaviour, INavigator
{
    [SerializeField]
    private double percentOfEnemiesUntilEvolve = 0.9;

    public INeuralNetwork neuralNetwork = null;
    public static double LastFitness = 0;

    internal double x = 0;

    internal float output = 0;
    internal int actualInvidiual;

    private PlayerController playerController;

    private bool evolved = false;

    private void Start()
    {
        playerController= GetComponent<PlayerController>();
    }

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

        if (inputs == null) return 0;
         
        output = (float)neuralNetwork.Predict(inputs)[0];

        return output;
    }

    private MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[] FillInputs()
    {
        var hits = GetHits("Sensors");

        if (hits == null || hits[0].transform == null)
            return null;

        double v = 30d;
        int qtdInputs = hits.Length + 1;
        var inputs = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input[qtdInputs];

        x = hits[0].transform.position.x / 9f;
        inputs[0] = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(x);

        for (var i = 0; i < hits.Length; i++)
        {
            inputs[i + 1] = new MyNeuralNetwork.Domain.Entities.Nets.IO.Inputs.Input(hits[i].distance / v);
        }
        return inputs;
    }

    public void Feedback(float points)
    {
        var gameController = GameObject.Find("GameController").GetComponent<GameController>();

        if(points >= percentOfEnemiesUntilEvolve && !evolved)
        {
            gameController.StopToFollowPlayer();
            evolved = true;
        } 

        LastFitness = points;

        if(neuralNetwork != null)
            neuralNetwork.Fitness = LastFitness;
    }

    private static RaycastHit2D[] GetHits(string name)
    {
        if(GameObject.Find(name) == null) return null;

        return GameObject.Find(name).GetComponent<MultipleSensorsController>().hits;
    }
}

