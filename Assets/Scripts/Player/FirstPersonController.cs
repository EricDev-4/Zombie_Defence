using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] float walkSpeed = 3.0f;
    [SerializeField] float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] float mouseSensitivity = 2.0f;
    [SerializeField] float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] CharacterController characterController;

    [Header("SFX")]
    [SerializeField] private AudioClip[] arrcClips;
    [SerializeField] private float footstepCooldown = 0.2f; // 발소리 재생 최소 간격
    AudioSource audioSource;
    private float lastFootstepTime = -999f; // 마지막 발소리 재생 시간

    enum arrNumber
    {
        walk  = 0,
    }
    
    
    [SerializeField] private Transform head;
    [SerializeField] Transform cinemachineCamera;
    [SerializeField] PlayerInputHandler playerInputHandler;
    private WeaponInventory weaponInventory;

    private Vector3 currentMovement;
    private float verticalRotation;
    

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1.0f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        audioSource = FindAnyObjectByType<AudioSource>();
        weaponInventory = FindAnyObjectByType<WeaponInventory>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection()
    {
        if (cinemachineCamera == null)
            return Vector3.zero;

        // 카메라의 forward와 right 벡터를 가져옴 (Y축은 0으로)
        Vector3 cameraForward = cinemachineCamera.forward;
        Vector3 cameraRight = cinemachineCamera.right;

        // Y축 제거 (수평 이동만)
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // 입력에 따라 이동 방향 계산
        Vector3 worldDirection = cameraForward * playerInputHandler.MovementInput.y
                               + cameraRight * playerInputHandler.MovementInput.x;

        return worldDirection.normalized;
    }


    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x =  worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        bool isMoving = playerInputHandler.MovementInput.magnitude > 0.1f;
        bool isRunning = isMoving && playerInputHandler.SprintTriggered;
        bool isWalking = isMoving && !playerInputHandler.SprintTriggered;

        Animator anim = weaponInventory != null ? weaponInventory.currentAnimator : null;
        if (anim != null)
        {
            anim.SetBool("walking", isWalking);
            anim.SetBool("running", isRunning);
        }
        
        if (characterController.isGrounded)
        {
            // Stick to ground
            currentMovement.y = -0.5f;

            // Jump if space is pressed
            if (playerInputHandler.JumpTriggered)
            {
                Debug.Log("Jump triggered");
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            // Apply gravity continuously when airborne
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        characterController.Move(currentMovement * Time.deltaTime);
        
    }

    private void HandleRotation()
    {
        if (cinemachineCamera == null) return;

        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity * Time.deltaTime;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity * Time.deltaTime;

        // 수평 회전: 플레이어 본체를 Y축으로 회전
        transform.Rotate(0, mouseXRotation, 0);

        // 수직 회전: head를 X축으로 회전 (자식인 팔도 함께 회전)
        verticalRotation = Mathf.Clamp(verticalRotation - mouseYRotation, -upDownLookRange, upDownLookRange);
        head.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // 애니메이션 이벤트에서 호출할 메서드
    public void PlayFootstepSound()
    {
        // 쿨다운 체크: 마지막 발소리 이후 충분한 시간이 지났는지 확인
        if (Time.time - lastFootstepTime < footstepCooldown)
            return;

        if (audioSource != null && arrcClips != null && arrcClips.Length > 0)
        {
            audioSource.PlayOneShot(arrcClips[(int)arrNumber.walk]);
            lastFootstepTime = Time.time;
        }
    }
}
