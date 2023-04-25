using Assets.Scripts.Entities.Rays;
using UnityEngine;

namespace Assets.Scripts.Entities.Sensors.Interfaces.Builder
{
    internal interface IHitGetter
    {
        RaycastHit2D GetHit(MyRay ray);
    }
}
