using AutoMapper;
using Core.Infra.Services.Persistences;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Layers;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Networks;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Neurons;
using MyNeuralNetwork.Domain.Dtos.Entities.Nets.Neurons.Parts;
using MyNeuralNetwork.Domain.Entities.Commons.Fields.Numerics;
using MyNeuralNetwork.Domain.Entities.Nets.Conversors;
using MyNeuralNetwork.Domain.Entities.Nets.Generators.Supports;
using MyNeuralNetwork.Domain.Entities.Nets.Layers;
using MyNeuralNetwork.Domain.Entities.Nets.Networks;
using MyNeuralNetwork.Domain.Entities.Nets.Networks.Circuits.Forward;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons.Parts;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using MyNeuralNetwork.Domain.Interfaces.Networks.Circuits.Forward;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Activations;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Parts;
using System.Linq.Expressions;
using System;

namespace Assets.Scripts.AI.Supports
{
    internal class PersistenceService
    {
        private NeuralNetworkPersistenceService _persistence;
        private DtoToNeuralNetwork _dtoToNnConversor;
        private Mapper _mapper;

        public string Path { get; set; } = "C:\\Projetos\\Unity\\SpaceShooter\\";
        public string FileName { get; set; } = "neural_network.txt";

        public PersistenceService()
        {
            _mapper = GenerateMapper();
            _persistence = new NeuralNetworkPersistenceService(_mapper);
            _dtoToNnConversor = new DtoToNeuralNetwork(new LayersLinker());

            _persistence.Path = Path;
            _persistence.FileName = FileName;
        }

        public void Save(NeuralNetwork neuralNetwork)
        {
            _persistence.Save(neuralNetwork);
        }

        public INeuralNetwork Load()
        {
            return _dtoToNnConversor.Convert(_persistence.Load());
        }

        private Mapper GenerateMapper()
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(delegate (IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<NeuralDoubleValue, double>().ConvertUsing((NeuralDoubleValue f) => f.Value);
                cfg.CreateMap<double, NeuralDoubleValue>().ConvertUsing((double f) => new NeuralDoubleValue(f));
                cfg.CreateMap<ICircuitForward, string>().ConvertUsing((ICircuitForward c) => c.GetType().Name);
                cfg.CreateMap<NeuralFloatValue, float>().ConvertUsing((NeuralFloatValue f) => f.Value);
                cfg.CreateMap<float, NeuralFloatValue>().ConvertUsing((float f) => new NeuralFloatValue(f));
                cfg.CreateMap<Neuron, NeuronDto>().ForMember((NeuronDto member) => member.Activator, delegate (IMemberConfigurationExpression<Neuron, NeuronDto, string> opt)
                {
                    opt.MapFrom((Neuron item) => item.Activation.GetType().FullName);
                });
                (cfg.CreateMap<IActivator, string>()).ConstructUsing((IActivator c) => c.GetType().FullName);
                cfg.CreateMap<ISynapseManager, SynapseManagerDto>();
                cfg.CreateMap<INeuralNetwork, NeuralNetworkDto>().ForMember((NeuralNetworkDto member) => member.CircuitForward, delegate (IMemberConfigurationExpression<INeuralNetwork, NeuralNetworkDto, string> opt)
                {
                    opt.MapFrom((INeuralNetwork item) => item.CircuitForward.GetType().FullName);
                });
                cfg.CreateMap<Layer, LayerDto>();
                cfg.CreateMap<Synapse, SynapseDto>().ForMember((SynapseDto s) => s.TargetGuid, delegate (IMemberConfigurationExpression<Synapse, SynapseDto, int> opt)
                {
                    opt.MapFrom((Synapse y) => y.NeuronSource.Index);
                });
            });
            LambdaExpression lambdaExpression = mapperConfiguration.BuildExecutionPlan(typeof(NeuralNetwork), typeof(NeuralNetworkDto));
            return new Mapper(mapperConfiguration);
        }
    }

}
