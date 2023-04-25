using UnityEngine;

namespace Assets.Scripts.Entities.Rays
{
    internal class MyRay
    {
        public Vector2 Direction { get; }
        public Ray Ray { get; }

        public MyRay(Vector2 direction, Ray ray)
        {
            Direction = direction;
            Ray = ray;
        }
    }
}
