using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color lowAmmoColor = Color.red;
    [SerializeField] private int lowAmmoThreshold = 10;

    private void Update()
    {
        if (weaponSystem == null || ammoText == null)
            return;

        AmmoData ammoData = weaponSystem.GetAmmoData();

        if (ammoData == null)
            return;

        // 탄약 텍스트 업데이트
        ammoText.text = $"{ammoData.CurrentMagazineAmmo} / {ammoData.CurrentReserveAmmo}";

        // 탄약이 적을 때 빨간색으로 변경
        if (ammoData.CurrentMagazineAmmo <= lowAmmoThreshold)
        {
            ammoText.color = lowAmmoColor;
        }
        else
        {
            ammoText.color = normalColor;
        }
    }
}
