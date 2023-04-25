using Assets.Scripts.Entities.Rays;

namespace Assets.Scripts.Entities.Sensors.Interfaces.Builder
{
    internal interface IDrawMaker : ILayerMaskDefiner
    {
        public ILayerMaskDefiner Draw(MyRay ray);
    }
}
