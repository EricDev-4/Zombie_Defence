using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct MagnificationLevel
{
    public float multiplier;
    public float targetFOV;

    public MagnificationLevel(float multiplier, float targetFOV)
    {
        this.multiplier = multiplier;
        this.targetFOV = targetFOV;
    }
}

public class WeaponSystem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject bulletPrefab;
    private Recoil _recoil;

    [Header("Ammo System")]
    [SerializeField] private AmmoData ammoData;
    
    [Header("Animation name Reference")]
     private string shooting = "shooting";
     private string reloading = "reloading";

    [Header("Animator Layer")]
    [SerializeField] private string reloadLayerName = "Reload Layer";
    private int reloadLayerIndex = -1;
    
    
    [Header("ADS")]
    [SerializeField] private Transform defaultPos;
    [SerializeField] private Transform adsPos;
    
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private Camera weaponCamera;

    [SerializeField] private float defaultFOV;

    [Header("Magnification Settings")]
    [SerializeField] private MagnificationLevel[] magnificationLevels = new MagnificationLevel[]
    {
        new MagnificationLevel(1.0f, 60f),
        new MagnificationLevel(1.5f, 42f),
        new MagnificationLevel(2.0f, 33f),
        new MagnificationLevel(3.0f, 23f),
        new MagnificationLevel(4.0f, 18f),
        new MagnificationLevel(6.0f, 12f)
    };

    [SerializeField] private int currentMagnificationIndex = 0;
    [SerializeField] private float zoomTransitionSpeed = 8f;
    private float targetFOV;

    [SerializeField] private float adsAnimationSpeed = 1f;
    private Transform weaponPos;


    [Header("Parameters")]
    [SerializeField] private float damage = 17.5f;
    [SerializeField] private float fireRate = 0.1f; // delay 대신 fireRate
    [SerializeField] private float range = 100f;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject EnemyImpact;
    [SerializeField] private GameObject BulletImpact;
    [SerializeField] private Transform firePoint;
    private float impactDuration = 3f;
    
    [Header("SFX")]
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioSource reloadAudioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;
    
    private float nextFireTime = 0f; // 코루틴 대신 시간 체크
    private RaycastHit hitInfo;
    private bool isReloading = false;
    
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        weaponPos = GetComponent<Transform>();
        _recoil = FindAnyObjectByType<Recoil>();
        
        
        defaultFOV = mainCamera.Lens.FieldOfView;

        targetFOV = defaultFOV;
        currentMagnificationIndex = 0;

        // muzzleFlash가 Inspector에서 할당되지 않은 경우에만 자동 할당
        if (muzzleFlash == null)
            muzzleFlash = GetComponentInChildren<ParticleSystem>();

        // Reload Layer 인덱스 찾기
        if (animator != null)
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                if (animator.GetLayerName(i) == reloadLayerName)
                {
                    reloadLayerIndex = i;
                    // 시작 시 Layer Weight를 0으로 설정 (비활성화)
                    animator.SetLayerWeight(reloadLayerIndex, 0f);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        // 재장전 입력 확인
        if (playerInputHandler.ReloadTriggered && !isReloading)
        {
            TryReload();
        }
        
        // 연사 가능 (홀드하면 계속 발사) - 달리는 중에는 사격 불가
        if (playerInputHandler.ShotTriggered && Time.time >= nextFireTime && !isReloading && !playerInputHandler.SprintTriggered)
        {
            if (ammoData.HasAmmoInMagazine())
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                PlayDryFireSound();
            }
        }
        AimDownSight();
        HandleMagnificationInput();
    }

    void Shoot()
    {
        // 탄약 소모
        ammoData.DecrementMagazineAmmo();
        _recoil.RecoilFire();
        // 레이캐스트 발사
        if (Physics.Raycast(firePoint.position,firePoint.forward, out hitInfo, range))
        {
           if(hitInfo.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hitInfo.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    // enemy hit
                    enemy.Damaged(damage);
                    GameObject enemyImpact = GameManager.instance.pool.Get(2);
                    Transform t= enemyImpact.transform;
                    t.position = hitInfo.point + hitInfo.normal * 0.001f;
                    t.rotation = Quaternion.LookRotation(-hitInfo.normal, Vector3.up);
                }
            }
           else
            {
                GameObject bulletHole = GameManager.instance.pool.Get(1);
                Transform t = bulletHole.transform;
                t.SetParent(hitInfo.transform);
                t.position = hitInfo.point + hitInfo.normal * 0.001f;
                t.rotation = Quaternion.LookRotation(-hitInfo.normal, Vector3.up);
            }
            //GameObject impact = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), hitInfo.transform);
            //Destroy(impact.gameObject , impactDuration);
        }
        if (animator != null)
            animator.SetTrigger("shooting");

        if (muzzleFlash != null)
            muzzleFlash.GetComponent<Transform>().localPosition = firePoint.localPosition;
            muzzleFlash.Play();

        // 총성 사운드 재생 (별도 AudioSource 사용)
        if (shootAudioSource != null && shootClip != null)
        {
            shootAudioSource.PlayOneShot(shootClip);
        }

        // 디버그 Ray 표시
        Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 0.5f);
    }
    
    void AimDownSight()
    {
        if (playerInputHandler.AdsTriggered && !playerInputHandler.SprintTriggered)
        {
            Debug.Log("ADS");
            weaponPos.localPosition = Vector3.Lerp(weaponPos.localPosition, adsPos.localPosition, adsAnimationSpeed * Time.deltaTime);
            weaponPos.localRotation = Quaternion.Slerp(weaponPos.localRotation , adsPos.localRotation, adsAnimationSpeed * Time.deltaTime);

            if (magnificationLevels != null && magnificationLevels.Length > 0 &&
                currentMagnificationIndex >= 0 && currentMagnificationIndex < magnificationLevels.Length)
            {
                targetFOV = magnificationLevels[currentMagnificationIndex].targetFOV;
            }
            else
            {
                targetFOV = defaultFOV * 0.6f;
            }
        }
        else
        {
            weaponPos.localPosition = Vector3.Lerp(weaponPos.localPosition, defaultPos.localPosition, adsAnimationSpeed * Time.deltaTime);
            weaponPos.localRotation = Quaternion.Slerp(weaponPos.localRotation , defaultPos.localRotation, adsAnimationSpeed * Time.deltaTime);

            targetFOV = defaultFOV;
            currentMagnificationIndex = 0;
        }

        SetFov(Mathf.Lerp(mainCamera.Lens.FieldOfView, targetFOV, zoomTransitionSpeed * Time.deltaTime));
    }

    void HandleMagnificationInput()
    {
        if (!playerInputHandler.AdsTriggered)
            return;

        if (magnificationLevels == null || magnificationLevels.Length == 0)
            return;

        float scrollInput = playerInputHandler.ZoomScrollInput;

        if (scrollInput > 0.1f)
        {
            currentMagnificationIndex++;
            if (currentMagnificationIndex >= magnificationLevels.Length)
                currentMagnificationIndex = 0;
        }
        else if (scrollInput < -0.1f)
        {
            currentMagnificationIndex--;
            if (currentMagnificationIndex < 0)
                currentMagnificationIndex = magnificationLevels.Length - 1;
        }
    }

    void SetFov(float fov)
    {
        var lens = mainCamera.Lens;
        lens.FieldOfView = fov;
        mainCamera.Lens = lens;
        
        weaponCamera.fieldOfView = fov;
    }


    void TryReload()
    {
        if (ammoData.IsMagazineFull())
        {
            Debug.Log("Magazine already full!");
            return;
        }

        if (!ammoData.HasReserveAmmo())
        {
            Debug.Log("No reserve ammo!");
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        // Reload Layer 활성화 (Weight를 1로)
        if (animator != null && reloadLayerIndex >= 0)
        {
            animator.SetLayerWeight(reloadLayerIndex, 1f);
        }

        // 재장전 애니메이션 트리거
        if (animator != null)
            animator.SetTrigger(reloading);

        // 재장전 사운드 재생 (별도 AudioSource 사용, Stop 없이)
        if (reloadAudioSource != null && reloadClip != null)
        {
            reloadAudioSource.PlayOneShot(reloadClip);
        }

        // 재장전 시간 대기
        yield return new WaitForSeconds(ammoData.ReloadDuration);

        // 탄약 재장전
        ammoData.Reload();

        // Reload Layer 비활성화 (Weight를 0으로)
        if (animator != null && reloadLayerIndex >= 0)
        {
            animator.SetLayerWeight(reloadLayerIndex, 0f);
        }

        isReloading = false;
    }

    void PlayDryFireSound()
    {
        Debug.Log("Click! Magazine is empty. Press R to reload.");
        // 나중에 빈 탄창 클릭 사운드 추가 가능
    }

    public AmmoData GetAmmoData()
    {
        return ammoData;
    }
}