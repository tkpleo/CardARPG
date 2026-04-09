/*
 * Written by: Will T
 * 
 * Abstract class for player components to inherit from.
 * Helps automatically set up the component with the player static class.
 */

using UnityEngine;

namespace Player
{
    public abstract class PlayerComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Player.RegisterComponent(this);
        }
    }
}
