/*
 * Written by: Will T
 * 
 * Central static class for all of the player's components.
 * Helps keep track of all the different components that make up the player and helps them communicate with each other.
 * Each component should be a subclass of PlayerComponent, and they can register themselves automatically with this class
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Player
{
    public static class Player
    {
        // Stores each different component instance via its type.
        private static readonly Dictionary<Type, PlayerComponent> componentsByType = new();

        // Register a component instance. If another instance of the same concrete type exists it will be replaced.
        public static void RegisterComponent(PlayerComponent component)
        {
            if (component == null) return;

            var type = component.GetType();
            componentsByType[type] = component;
        }

        // Try to get a registered component by its concrete type T (or a subclass).
        public static bool TryGetComponent<T>(out T component) where T : PlayerComponent
        {
            if (componentsByType.TryGetValue(typeof(T), out var found) && found is T typed)
            {
                component = typed;
                return true;
            }

            component = null;
            return false;
        }

        // Convenience getter (returns null if not found).
        public static T GetComponent<T>() where T : PlayerComponent
        {
            return TryGetComponent<T>(out var c) ? c : null;
        }

        // Enumerate all registered components.
        public static IEnumerable<PlayerComponent> GetAllComponents()
        {
            // Return a copy to avoid external modification of internal collection.
            return componentsByType.Values.ToList();
        }
    }
}
