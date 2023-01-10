//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerActionMap"",
            ""id"": ""711e618a-c435-4fab-973d-3abdb3adac8e"",
            ""actions"": [
                {
                    ""name"": ""MoveForward"",
                    ""type"": ""Button"",
                    ""id"": ""7658cd35-43fb-4060-83a9-c3ab1d0e4caa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveBackwards"",
                    ""type"": ""Button"",
                    ""id"": ""0af961a8-b8d9-426b-a1c9-b96b54a34517"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TurnLeft"",
                    ""type"": ""Button"",
                    ""id"": ""9f152f0b-4c6b-4114-92b1-77a62016ab18"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TurnRight"",
                    ""type"": ""Button"",
                    ""id"": ""58a86fe3-b562-4f33-b9b7-182a0e318f24"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""d3b7f8a9-8f9f-4aa2-944d-07f18ff7192f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a6c3543a-4218-4084-ac9c-e32e81dbc6d6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8eafc6ba-0d86-489e-97ad-fe21a2af9dd3"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""00700056-ce83-4492-85fd-e6d0f819b266"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c3e8df52-a69b-4980-9790-d6b604b7d8ef"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBackwards"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7dba2e54-920d-4aef-b58c-40ab00c93be0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c727b72-47f4-4dcd-b533-d2fed9f856f8"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f0fc8097-db01-4648-948d-228bae8b6bb0"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f403a2d4-5e64-49ee-9e9f-a4ed2c0b2ba7"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TurnRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24c0e15e-27d7-4468-a86c-d4fa10cc97ec"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c654a70b-1468-4a7e-ac93-f70007de3d01"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerActionMap
        m_PlayerActionMap = asset.FindActionMap("PlayerActionMap", throwIfNotFound: true);
        m_PlayerActionMap_MoveForward = m_PlayerActionMap.FindAction("MoveForward", throwIfNotFound: true);
        m_PlayerActionMap_MoveBackwards = m_PlayerActionMap.FindAction("MoveBackwards", throwIfNotFound: true);
        m_PlayerActionMap_TurnLeft = m_PlayerActionMap.FindAction("TurnLeft", throwIfNotFound: true);
        m_PlayerActionMap_TurnRight = m_PlayerActionMap.FindAction("TurnRight", throwIfNotFound: true);
        m_PlayerActionMap_Shoot = m_PlayerActionMap.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerActionMap
    private readonly InputActionMap m_PlayerActionMap;
    private IPlayerActionMapActions m_PlayerActionMapActionsCallbackInterface;
    private readonly InputAction m_PlayerActionMap_MoveForward;
    private readonly InputAction m_PlayerActionMap_MoveBackwards;
    private readonly InputAction m_PlayerActionMap_TurnLeft;
    private readonly InputAction m_PlayerActionMap_TurnRight;
    private readonly InputAction m_PlayerActionMap_Shoot;
    public struct PlayerActionMapActions
    {
        private @InputActions m_Wrapper;
        public PlayerActionMapActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveForward => m_Wrapper.m_PlayerActionMap_MoveForward;
        public InputAction @MoveBackwards => m_Wrapper.m_PlayerActionMap_MoveBackwards;
        public InputAction @TurnLeft => m_Wrapper.m_PlayerActionMap_TurnLeft;
        public InputAction @TurnRight => m_Wrapper.m_PlayerActionMap_TurnRight;
        public InputAction @Shoot => m_Wrapper.m_PlayerActionMap_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionMapActions instance)
        {
            if (m_Wrapper.m_PlayerActionMapActionsCallbackInterface != null)
            {
                @MoveForward.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveForward;
                @MoveForward.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveForward;
                @MoveForward.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveForward;
                @MoveBackwards.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveBackwards;
                @MoveBackwards.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveBackwards;
                @MoveBackwards.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMoveBackwards;
                @TurnLeft.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnLeft;
                @TurnLeft.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnLeft;
                @TurnLeft.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnLeft;
                @TurnRight.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnRight;
                @TurnRight.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnRight;
                @TurnRight.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnTurnRight;
                @Shoot.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_PlayerActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveForward.started += instance.OnMoveForward;
                @MoveForward.performed += instance.OnMoveForward;
                @MoveForward.canceled += instance.OnMoveForward;
                @MoveBackwards.started += instance.OnMoveBackwards;
                @MoveBackwards.performed += instance.OnMoveBackwards;
                @MoveBackwards.canceled += instance.OnMoveBackwards;
                @TurnLeft.started += instance.OnTurnLeft;
                @TurnLeft.performed += instance.OnTurnLeft;
                @TurnLeft.canceled += instance.OnTurnLeft;
                @TurnRight.started += instance.OnTurnRight;
                @TurnRight.performed += instance.OnTurnRight;
                @TurnRight.canceled += instance.OnTurnRight;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public PlayerActionMapActions @PlayerActionMap => new PlayerActionMapActions(this);
    public interface IPlayerActionMapActions
    {
        void OnMoveForward(InputAction.CallbackContext context);
        void OnMoveBackwards(InputAction.CallbackContext context);
        void OnTurnLeft(InputAction.CallbackContext context);
        void OnTurnRight(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}