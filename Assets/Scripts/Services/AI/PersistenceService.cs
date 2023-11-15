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
using MyNeuralNetwork.Domain.Entities.Nets.Neurons;
using MyNeuralNetwork.Domain.Entities.Nets.Neurons.Parts;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using MyNeuralNetwork.Domain.Interfaces.Networks.Circuits.Forward;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Activations;
using MyNeuralNetwork.Domain.Interfaces.Neurons.Parts;
using UnityEngine;

namespace Assets.Scripts.AI.Supports
{
    internal class PersistenceService
    {
        private NeuralNetworkPersistenceService _persistence;
        private DtoToNeuralNetwork _dtoToNnConversor;
        private Mapper _mapper;

        public string Path { get; set; } = Application.dataPath;
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
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NeuralDoubleValue, double>().ConvertUsing(f => f.Value);
                cfg.CreateMap<double, NeuralDoubleValue>().ConvertUsing(f => new NeuralDoubleValue(f));

                cfg.CreateMap<ICircuitForward, string>().ConvertUsing(c => c.GetType().Name);

                cfg.CreateMap<NeuralFloatValue, float>().ConvertUsing(f => f.Value);
                cfg.CreateMap<float, NeuralFloatValue>().ConvertUsing(f => new NeuralFloatValue(f));

                cfg.CreateMap<Neuron, NeuronDto>()
                    .ForMember(member => member.Activator, opt => opt.MapFrom(item => item.Activation.GetType().FullName));

                cfg.CreateMap<IActivator, string>().ConstructUsing(c => c.GetType().FullName);

                cfg.CreateMap<ISynapseManager, SynapseManagerDto>();

                cfg.CreateMap<INeuralNetwork, NeuralNetworkDto>()
                    .ForMember(member => member.CircuitForward, opt => opt.MapFrom(item => item.CircuitForward.GetType().FullName));

                cfg.CreateMap<Layer, LayerDto>();
                cfg.CreateMap<Synapse, SynapseDto>().ForMember(s => s.TargetGuid, opt => opt.MapFrom((y) => y.NeighborNeuron.Index));
            });

            var executionPlan = mapperConfiguration.BuildExecutionPlan(typeof(NeuralNetwork), typeof(NeuralNetworkDto));

            return new Mapper(mapperConfiguration);
        }
    }

}
