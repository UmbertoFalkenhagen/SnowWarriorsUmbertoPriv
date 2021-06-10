// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""IngameControls"",
            ""id"": ""4958be61-e186-4c66-8688-e0e9fc0d4cd9"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""38bdd1bc-3723-4bf0-82fe-5ec2f25d0832"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7c78b447-fae2-458a-89f2-bf8ea2b966c7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RollIceBall"",
                    ""type"": ""PassThrough"",
                    ""id"": ""7c4ec215-85f8-4984-b30f-211cd69287c1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateToMousePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4d8924a6-80ca-4677-8888-85fff8baf646"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASDControls"",
                    ""id"": ""0b0a5dd8-a2dd-498f-89c7-3b7dc3344e17"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3dd6bd67-811a-4b6a-9843-c63386618c72"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f8ef18b3-6baa-4def-b056-b18127e2d6a3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""713342a2-ddbe-4439-a0df-685a84469a3e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e6f10e9e-0b77-4225-8efc-8fafb9bf47c3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""052d505e-b677-4f53-9308-5c6f122d142a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""72720664-1885-4553-9408-22e8e8f16514"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RollIceBall"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14e6a276-0a76-4ca2-bd7e-196fe58fc169"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateToMousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // IngameControls
        m_IngameControls = asset.FindActionMap("IngameControls", throwIfNotFound: true);
        m_IngameControls_Movement = m_IngameControls.FindAction("Movement", throwIfNotFound: true);
        m_IngameControls_Shoot = m_IngameControls.FindAction("Shoot", throwIfNotFound: true);
        m_IngameControls_RollIceBall = m_IngameControls.FindAction("RollIceBall", throwIfNotFound: true);
        m_IngameControls_RotateToMousePosition = m_IngameControls.FindAction("RotateToMousePosition", throwIfNotFound: true);
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

    // IngameControls
    private readonly InputActionMap m_IngameControls;
    private IIngameControlsActions m_IngameControlsActionsCallbackInterface;
    private readonly InputAction m_IngameControls_Movement;
    private readonly InputAction m_IngameControls_Shoot;
    private readonly InputAction m_IngameControls_RollIceBall;
    private readonly InputAction m_IngameControls_RotateToMousePosition;
    public struct IngameControlsActions
    {
        private @PlayerControls m_Wrapper;
        public IngameControlsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_IngameControls_Movement;
        public InputAction @Shoot => m_Wrapper.m_IngameControls_Shoot;
        public InputAction @RollIceBall => m_Wrapper.m_IngameControls_RollIceBall;
        public InputAction @RotateToMousePosition => m_Wrapper.m_IngameControls_RotateToMousePosition;
        public InputActionMap Get() { return m_Wrapper.m_IngameControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(IngameControlsActions set) { return set.Get(); }
        public void SetCallbacks(IIngameControlsActions instance)
        {
            if (m_Wrapper.m_IngameControlsActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnMovement;
                @Shoot.started -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnShoot;
                @RollIceBall.started -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRollIceBall;
                @RollIceBall.performed -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRollIceBall;
                @RollIceBall.canceled -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRollIceBall;
                @RotateToMousePosition.started -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRotateToMousePosition;
                @RotateToMousePosition.performed -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRotateToMousePosition;
                @RotateToMousePosition.canceled -= m_Wrapper.m_IngameControlsActionsCallbackInterface.OnRotateToMousePosition;
            }
            m_Wrapper.m_IngameControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @RollIceBall.started += instance.OnRollIceBall;
                @RollIceBall.performed += instance.OnRollIceBall;
                @RollIceBall.canceled += instance.OnRollIceBall;
                @RotateToMousePosition.started += instance.OnRotateToMousePosition;
                @RotateToMousePosition.performed += instance.OnRotateToMousePosition;
                @RotateToMousePosition.canceled += instance.OnRotateToMousePosition;
            }
        }
    }
    public IngameControlsActions @IngameControls => new IngameControlsActions(this);
    public interface IIngameControlsActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnRollIceBall(InputAction.CallbackContext context);
        void OnRotateToMousePosition(InputAction.CallbackContext context);
    }
}
