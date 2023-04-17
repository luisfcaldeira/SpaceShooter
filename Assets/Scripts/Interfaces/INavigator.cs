using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface INavigator
    {
        Vector2 Move();
        void ElapsedTime(float elapsedTime);
    }
}
