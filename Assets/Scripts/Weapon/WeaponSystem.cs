using System;
using System.Collections;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Ammo System")]
    [SerializeField] private AmmoData ammoData;
    
    [Header("Animation name Reference")]
     private string shooting = "shooting";
     private string reloading = "reloading";

    [Header("Animator Layer")]
    [SerializeField] private string reloadLayerName = "Reload Layer";
    private int reloadLayerIndex = -1;
    
    [Header("Position")]
    [SerializeField] private Transform firePoint;
    
    [Header("Parameters")]
    [SerializeField] private float fireRate = 0.1f; // delay 대신 fireRate
    [SerializeField] private float range = 100f;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
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
    }

    void Shoot()
    {
        // 탄약 소모
        ammoData.DecrementMagazineAmmo();

        // 레이캐스트 발사
        if (Physics.Raycast(firePoint.position,firePoint.forward, out hitInfo, range))
        {
            GameObject impact = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), hitInfo.transform);

            Destroy(impact.gameObject , impactDuration);
        }
        if (animator != null)
            animator.SetTrigger("shooting");

        if (muzzleFlash != null)
            muzzleFlash.Play();

        // 총성 사운드 재생 (별도 AudioSource 사용)
        if (shootAudioSource != null && shootClip != null)
        {
            shootAudioSource.PlayOneShot(shootClip);
        }

        // 디버그 Ray 표시
        Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 0.5f);
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