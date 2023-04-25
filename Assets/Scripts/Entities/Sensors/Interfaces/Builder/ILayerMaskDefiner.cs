using UnityEngine;

namespace Assets.Scripts.Entities.Sensors.Interfaces.Builder
{
    internal interface ILayerMaskDefiner
    {
        IHitGetter WithLayerMask(LayerMask layerMask);
    }
}
