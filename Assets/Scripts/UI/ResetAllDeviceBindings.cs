using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string targetControlScheme;

    public void ResetAllBindings()
    {
        foreach(InputActionMap actionMap in inputActions.actionMaps)
        {
            foreach(InputAction action in actionMap.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }

    public void ResetControlSchemeBindings()
    {
        foreach(InputActionMap actionMap in inputActions.actionMaps)
        {
            foreach(InputAction action in actionMap.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(targetControlScheme));
            }
        }
    }
}
