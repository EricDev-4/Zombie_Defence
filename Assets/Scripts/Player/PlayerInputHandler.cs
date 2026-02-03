using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action name Reference")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string shot = "Shot";
    [SerializeField] private string reload = "Reload";
    [SerializeField] private string ads = "Ads";
    [SerializeField] private string zoomScroll = "ZoomScroll";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction shotAction;
    private InputAction reloadAction;
    private InputAction adsAction;
    private InputAction zoomScrollAction;

    public Vector2 MovementInput {get; private set;}
    public Vector2 RotationInput {get; private set;}
    public bool JumpTriggered {get; private set;}
    public bool SprintTriggered {get; private set;}
    public bool ShotTriggered {get; private set;}
    public bool ReloadTriggered {get; private set;}
    public bool AdsTriggered {get; private set;}
    public float ZoomScrollInput {get; private set;}



    void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

        movementAction= mapReference.FindAction(movement);
        rotationAction= mapReference.FindAction(rotation);
        jumpAction= mapReference.FindAction(jump);
        sprintAction= mapReference.FindAction(sprint);
        shotAction= mapReference.FindAction(shot);
        reloadAction= mapReference.FindAction(reload);
        adsAction= mapReference.FindAction(ads);
        zoomScrollAction= mapReference.FindAction(zoomScroll);

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo =>  JumpTriggered = false;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo =>  SprintTriggered = false;

        shotAction.performed += inputInfo => ShotTriggered = true;
        shotAction.canceled += inputInfo =>  ShotTriggered = false;

        reloadAction.performed += inputInfo => ReloadTriggered = true;
        reloadAction.canceled += inputInfo =>  ReloadTriggered = false;
        
        adsAction.performed += inputInfo => AdsTriggered = true;
        adsAction.canceled += inputInfo =>  AdsTriggered = false;
    }

    private void Update()
    {
        // 마우스 델타는 매 프레임 직접 읽어야 함
        RotationInput = rotationAction.ReadValue<Vector2>();
        ZoomScrollInput = zoomScrollAction.ReadValue<float>();
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }
    
    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }
}
