using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    [SerializeField] WeaponSystem[] weaponArray;

    private int currentIndex = 0;
    public GameObject currentWeapon => weaponArray[currentIndex].transform.parent.gameObject;
    public Animator currentAnimator => weaponArray[currentIndex].GetComponentInChildren<Animator>();

    void Start()
    {
        // 모든 무기 비활성화 후 첫 번째 무기만 활성화
        for (int i = 0; i < weaponArray.Length; i++)
        {
            weaponArray[i].transform.parent.gameObject.SetActive(i == 0);
        }
    }

    void Update()
    {
        for (int i = 0; i < weaponArray.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
                break;
            }
        }
    }

    void SwitchWeapon(int index)
    {
        if (index == currentIndex) return;
        if (index < 0 || index >= weaponArray.Length) return;

        currentWeapon.SetActive(false);
        currentIndex = index;
        currentWeapon.SetActive(true);
    }
}