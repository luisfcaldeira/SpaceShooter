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

    GeneticTrainer geneticTrainer;

    private static double LastFitness = double.MaxValue;

    void Start()
    {
        var ngen = new NeuronGenerator();
        ngen.WeightConfiguration.SetMaxAndMin(-1, 1);
        ngen.BiasConfiguration.SetMaxAndMin(-1, 1);


        geneticTrainer = new GeneticTrainer(new Mutater() { 
        ChanceOfMutate = 0.9 });

        nnGen = new NNGenerator(ngen, new LayersLinker());

        neuralNetworks = new List<INeuralNetwork>();

        for(var i = 0; i < numberOfNeuralNetworks; i++)
        {
            neuralNetworks.Add(nnGen.Generate<SynapseManager>(new int[] { 4, 20, 20, 1 }
            , new IActivator[] { new Relu(), new Tanh(), new Tanh(), new Tanh()}));
        }
    }

    void Update()
    {
        var gameController = GameObject.Find("GameController").GetComponent<GameController>();

        var player = gameController.instantiatedPlayer;

        if (player != null)
        {
            var playerController = player.GetComponent<PlayerController>();
            var navigator = playerController.navigator as AINavigator;

            if(navigator != null && navigator.neuralNetwork != null)
            {
                LastFitness = AINavigator.LastFitness;
            }

            if (navigator != null && navigator.neuralNetwork == null)
            {
                ResetEpoch();
                navigator.neuralNetwork = neuralNetworks[actualNeural++];
            }
        }
    }

    private void ResetEpoch()
    {
        var bestNet = (NeuralNetwork)GeneticTrainer.GetTheBestOne(neuralNetworks);

        if (actualNeural == numberOfNeuralNetworks && actualEpoch < numberOfEpochs)
        {
            
            geneticTrainer.ToMutate(neuralNetworks);
            actualNeural = 0;
            actualEpoch++;

            Debug.Log($"Epoch reached: {actualEpoch}/{numberOfEpochs}. Best fitness: {bestNet.Fitness}");

        }

        if (actualEpoch >= numberOfEpochs || LastFitness <= 0.1)
        {
            var persistence = new NeuralNetworkPersistenceService(GenerateMapper());
            persistence.Path = "C:\\Projetos\\Unity\\SpaceShooter\\";
            persistence.FileName = "neural_network.txt";

            NeuralNetwork neuralNetwork = (NeuralNetwork)GeneticTrainer.GetTheBestOne(neuralNetworks);
            persistence.Save(neuralNetwork);
            Debug.Log($"Number of epochs reached. Fitness: {neuralNetwork.Fitness}");
            Debug.Break();
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

