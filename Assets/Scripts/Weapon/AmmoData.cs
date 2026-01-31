using UnityEngine;

[System.Serializable]
public class AmmoData
{
    [Header("Magazine Settings")]
    [SerializeField] private int magazineCapacity = 30;
    [SerializeField] private int currentMagazineAmmo = 30;

    [Header("Reserve Ammo Settings")]
    [SerializeField] private int maxReserveAmmo = 90;
    [SerializeField] private int currentReserveAmmo = 90;

    [Header("Reload Settings")]
    [SerializeField] private float reloadDuration = 2.0f;

    public int MagazineCapacity => magazineCapacity;
    public int CurrentMagazineAmmo => currentMagazineAmmo;
    public int MaxReserveAmmo => maxReserveAmmo;
    public int CurrentReserveAmmo => currentReserveAmmo;
    public float ReloadDuration => reloadDuration;

    /// <summary>
    /// Check if there is ammo in the magazine
    /// </summary>
    public bool HasAmmoInMagazine()
    {
        return currentMagazineAmmo > 0;
    }

    /// <summary>
    /// Check if there is reserve ammo available
    /// </summary>
    public bool HasReserveAmmo()
    {
        return currentReserveAmmo > 0;
    }

    /// <summary>
    /// Check if the magazine is full
    /// </summary>
    public bool IsMagazineFull()
    {
        return currentMagazineAmmo >= magazineCapacity;
    }

    /// <summary>
    /// Decrease magazine ammo by 1 when shooting
    /// </summary>
    public void DecrementMagazineAmmo()
    {
        if (currentMagazineAmmo > 0)
        {
            currentMagazineAmmo--;
        }
    }

    /// <summary>
    /// Reload the magazine from reserve ammo
    /// </summary>
    public void Reload()
    {
        if (!HasReserveAmmo())
        {
            Debug.Log("No reserve ammo to reload!");
            return;
        }

        int ammoNeeded = magazineCapacity - currentMagazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

        currentMagazineAmmo += ammoToReload;
        currentReserveAmmo -= ammoToReload;

        Debug.Log($"Reloaded {ammoToReload} rounds. Magazine: {currentMagazineAmmo}/{magazineCapacity}, Reserve: {currentReserveAmmo}/{maxReserveAmmo}");
    }
}
