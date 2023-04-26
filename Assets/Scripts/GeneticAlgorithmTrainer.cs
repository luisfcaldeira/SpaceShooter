using Assets.Scripts.AI.Supports;
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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    [SerializeField]
    internal int numberOfInputNeurons = 40;

    IGeneticTrainer geneticTrainer;

    private GameController gameController;
    private PersistenceService persistence;
    private double bestFitness = 0;


    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        persistence = new PersistenceService();
        var ngen = new NeuronGenerator();
        ngen.WeightConfiguration.SetMaxAndMin(-1, 1);
        ngen.BiasConfiguration.SetMaxAndMin(-1, 1);

        geneticTrainer = new SimpleTrainer(new Mutater(0.9, -0.5, 0.5), true);

        nnGen = new NNGenerator(ngen, new LayersLinker());

        neuralNetworks = new List<INeuralNetwork>();

        for(var i = 0; i < numberOfNeuralNetworks; i++)
        {
            int[] layers = GenerateLayerFormat();

            neuralNetworks.Add(nnGen.Generate<SynapseManager, Tanh>(layers));
        }
    }

    private int[] GenerateLayerFormat()
    {
        var qtdLayers = 5;
        var qtdNeurons = 128;
        var qtdNeuronsIn = numberOfInputNeurons + 1;
        var qtdNeuronsOut = 1;

        var layers = new int[qtdLayers];
        layers[0] = qtdNeuronsIn;
        for (var j = 1; j < qtdLayers - 1; j++)
        {
            layers[j] = qtdNeurons;
        }
        layers[qtdLayers - 1] = qtdNeuronsOut;
        return layers;
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
                if (bestFitness < navigator.neuralNetwork.Fitness)
                    bestFitness = navigator.neuralNetwork.Fitness;
            }

            if (navigator != null && navigator.neuralNetwork == null)
            {
                ResetEpoch();
                navigator.neuralNetwork = neuralNetworks[actualNeural++];
                navigator.actualInvidiual = actualNeural;
            }
        }
    }

    private void UpdateStatus()
    {
        var bestNet = (NeuralNetwork)geneticTrainer.GetTheBestOne(neuralNetworks);
        if(gameController.status != null)
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

        if (actualEpoch >= numberOfEpochs || neuralNetwork.Fitness > 0.9)
        {
            persistence.Save(neuralNetwork);
            
            gameController.status.text = ($"Number of epochs reached. Fitness: {neuralNetwork.Fitness}. Saved in {persistence.Path}.");
            gameController.EndGame();
        }
    }

}

