using Assets.Scripts.Interfaces;
using UnityEngine;


public class UserNavigator : MonoBehaviour, INavigator
{
    public void Feedback(float elapsedTime)
    {
        
    }

    public Vector2 Move()
    {
        Vector2 newVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        newVelocity.Normalize();

        return newVelocity;
    }
}

