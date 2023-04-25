using Assets.Scripts.Entities.Rays;
using Assets.Scripts.Entities.Sensors.Interfaces.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities.Sensors
{
    internal class Sensor : IDrawMaker, ILayerMaskDefiner, IHitGetter
    {
        float _distance = 0;
        private static Sensor _me;
        private LayerMask _layerMask;

        public Sensor() : this(50)
        { 
        }

        public Sensor(float distance)
        {
            _distance = distance;
        }

        public static IDrawMaker WithDistance(float distance)
        {
            _me = new Sensor(distance);
            return _me;
        }

        public static IDrawMaker WithDefaultDistance()
        {
            _me = new Sensor();
            return _me;
        }

        public ILayerMaskDefiner Draw(MyRay ray)
        {
            Debug.DrawRay(ray.Ray.origin, ray.Direction);

            return _me;
        }

        public IHitGetter WithLayerMask(LayerMask layerMask)
        {
            _layerMask = layerMask;
            return _me;
        }

        public RaycastHit2D GetHit(MyRay ray)
        {
            return Physics2D.Raycast(ray.Ray.origin, ray.Direction, _distance, _layerMask);
        }

    }
}
