using AutoMapper;
using Core.Infra.Services.Persistences;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Layers;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Networks;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Neurons;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Neurons.Parts;
using MyNeuralNetwork.Domain.Entities.Commons.Fields.Numerics;
using MyNeuralNetwork.Domain.Entities.Nets.Generators;
using MyNeuralNetwork.Domain.Entities.Nets.Generators.Supports;
using MyNeuralNetwork.Domain.Entities.Nets.Layers;
using MyNeuralNetwork.Domain.Entities.Nets.Networks;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons.Activations;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons.Parts;
using MyNeuralNetwork.Domain.Entities.Nets.Trainers.Genetics;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using MyNeuralNetwork.Domain.Interfaces.Networks.Circuits.Forward;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Activations;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Parts;
using MyNeuralNetwork.Domain.Interfaces.Trainers.Genetics;
using System.Collections.Generic;
using UnityEngine;

internal class GeneticAlgorithmTrainer : MonoBehaviour
{
    private List<INeuralNetwork> neuralNetworks;
    
    NNGenerator nnGen;

    [SerializeField]
    private int numberOfNeuralNetworks = 10;

    [SerializeField]
    private int numberOfEpochs = 5000;

    private int actualNeural = 0;
    private int actualEpoch = 0;

    IGeneticTrainer geneticTrainer;

    private static double LastFitness = 0;

    private GameController gameController;
    private double bestFitness = 0;

    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();

        var ngen = new NeuronGenerator();
        ngen.WeightConfiguration.SetMaxAndMin(-1, 1);
        ngen.BiasConfiguration.SetMaxAndMin(-1, 1);

        geneticTrainer = new SimpleTrainer(new Mutater(0.5, -1, 1), true);

        nnGen = new NNGenerator(ngen, new LayersLinker());

        neuralNetworks = new List<INeuralNetwork>();

        for(var i = 0; i < numberOfNeuralNetworks; i++)
        {
            neuralNetworks.Add(nnGen.Generate<SynapseManager>(new int[] { 6, 120, 120, 120, 120, 1 } 
            , new IActivator[] { new Relu(), new Sigmoid(), new Tanh(), new Tanh(), new Tanh(), new Tanh() }));
        }
    }

    void Update()
    {
        var player = gameController.instantiatedPlayer;
        UpdateStatus();

        if (player != null)
        {
            var playerController = player.GetComponent<PlayerController>();
            var navigator = playerController.navigator as AINavigator;

            if (navigator != null && navigator.neuralNetwork != null)
            {
                LastFitness = AINavigator.LastFitness;
                if (bestFitness < navigator.neuralNetwork.Fitness)
                    bestFitness = navigator.neuralNetwork.Fitness;
            }

            if (navigator != null && navigator.neuralNetwork == null)
            {
                ResetEpoch();
                navigator.neuralNetwork = neuralNetworks[actualNeural++];
            }
        }
    }

    private void UpdateStatus()
    {
        var bestNet = (NeuralNetwork)geneticTrainer.GetTheBestOne(neuralNetworks);
        gameController.status.text = ($"Epoch reached: {actualEpoch}/{numberOfEpochs}. Last best fitness of generations: {bestNet.Fitness}. Better fitness in anytime: {bestFitness}.");
    }

    private void ResetEpoch()
    {
        NeuralNetwork neuralNetwork = (NeuralNetwork)geneticTrainer.GetTheBestOne(neuralNetworks);

        if (actualNeural == numberOfNeuralNetworks && actualEpoch < numberOfEpochs)
        {
            geneticTrainer.ToMutate(neuralNetworks);
            actualNeural = 0;
            actualEpoch++;
        }

        if (actualEpoch >= numberOfEpochs || LastFitness > 0.9)
        {
            var persistence = new NeuralNetworkPersistenceService(GenerateMapper());
            persistence.Path = "C:\\Projetos\\Unity\\SpaceShooter\\";
            persistence.FileName = "neural_network.txt";

            persistence.Save(neuralNetwork);
            
            gameController.status.text = ($"Number of epochs reached. Fitness: {neuralNetwork.Fitness}. Saved in {persistence.Path}.");
            gameController.EndGame();
        }
    }

    private static Mapper GenerateMapper()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<NeuralDoubleValue, double>().ConvertUsing(f => f.Value);
            cfg.CreateMap<double, NeuralDoubleValue>().ConvertUsing(f => new NeuralDoubleValue(f));

            cfg.CreateMap<ICircuitForward, string>().ConvertUsing(c => c.GetType().Name);

            cfg.CreateMap<NeuralFloatValue, float>().ConvertUsing(f => f.Value);
            cfg.CreateMap<float, NeuralFloatValue>().ConvertUsing(f => new NeuralFloatValue(f));

            cfg.CreateMap<Neuron, NeuronDto>();

            cfg.CreateMap<IActivator, string>().ConstructUsing(c => c.GetType().FullName);

            cfg.CreateMap<ISynapseManager, SynapseManagerDto>();

            cfg.CreateMap<INeuralNetwork, NeuralNetworkDto>();

            cfg.CreateMap<Layer, LayerDto>();
            cfg.CreateMap<Synapse, SynapseDto>().ForMember(s => s.TargetGuid, opt => opt.MapFrom((x) => x.NeuronSource.Index));
        });

        return new Mapper(mapperConfiguration);
    }
}

