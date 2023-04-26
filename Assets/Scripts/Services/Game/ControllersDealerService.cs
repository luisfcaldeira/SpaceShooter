using System;
using UnityEngine;

namespace Assets.Scripts.Services.Game
{
    internal class ControllersDealerService
    {
        public GameController GetGameController()
        {
            return GameObject.Find("GameController")
                .GetComponent<GameController>();
        }

        public PlayerController GetPlayerController()
        {
            var player = GetGameController().instantiatedPlayer;
            ThrowExceptionIfNull(player, typeof(PlayerController).Name);

            var playerController = player.GetComponent<PlayerController>();
            return playerController;
        }

        public AINavigator GetAINavigator()
        {
            var playerController = GetPlayerController();
            
            var navigator = playerController.navigator as AINavigator;
            ThrowExceptionIfNull(navigator, typeof(AINavigator).Name);
            return navigator;
        }

        private static void ThrowExceptionIfNull(object obj, string whatWasNotFound)
        {
            if (obj == null)
                throw new Exception($"{whatWasNotFound} was not found.");
        }

    }
}
