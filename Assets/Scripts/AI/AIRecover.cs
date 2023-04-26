using Assets.Scripts.AI.Supports;
using Assets.Scripts.Services.Game;
using MyNeuralNetwork.Domain.Interfaces.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class AIRecover : MonoBehaviour
    {
        private PersistenceService _persistenceService;
        private ControllersDealerService _controllersDealerService;
        private INeuralNetwork _neuralNetwork;

        private void Start()
        {
            _persistenceService = new PersistenceService();
            _controllersDealerService = new ControllersDealerService();
            _neuralNetwork = _persistenceService.Load();
        }

        private void Update()
        {
            try
            {
                var navigator = _controllersDealerService.GetAINavigator();
                
                if(navigator.neuralNetwork == null)
                {
                    navigator.neuralNetwork = _neuralNetwork;
                }
            } catch(Exception)
            {

            }
        }
    }
}
