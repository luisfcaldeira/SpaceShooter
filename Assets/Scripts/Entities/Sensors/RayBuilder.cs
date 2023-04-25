using Assets.Scripts.Entities.Rays;
using Assets.Scripts.Entities.Sensors.Interfaces;
using Assets.Scripts.Entities.Sensors.Interfaces.Builder;
using UnityEngine;

namespace Assets.Scripts.Entities.Sensors
{
    internal class RayBuilder : IHorizontalDirection, IVerticalDirection, IBuilder
    {
        private static Vector2 _origin;
        private static RayBuilder _me;
        private float _horizontalDirection;
        private float _verticalDirection;

        public static IHorizontalDirection WithOrigin(Vector2 origin)
        {
            _origin = origin;
            _me = new RayBuilder();
            return _me;
        }

        public IVerticalDirection WithHorizontalDirection(float v)
        {
            _horizontalDirection = v;
            return _me;
        }

        public IBuilder WithVerticalDirection(float v)
        {
            _verticalDirection = v;
            return _me;
        }

        public MyRay Build()
        {
            var direction = new Vector2(_horizontalDirection, _verticalDirection);

            var ray = new Ray(_origin, direction);
            return new MyRay(direction, ray);
        }
    }
}
