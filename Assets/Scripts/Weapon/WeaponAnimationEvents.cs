using UnityEngine;

/// <summary>
/// 무기 애니메이션 이벤트를 받아서 FirstPersonController로 전달하는 스크립트
/// 무기 GameObject에 붙여서 사용
/// </summary>
public class WeaponAnimationEvents : MonoBehaviour
{
    private FirstPersonController controller;

    void Start()
    {
        // 부모에서 FirstPersonController 찾기
        controller = GetComponentInParent<FirstPersonController>();

        if (controller == null)
        {
            Debug.LogWarning("WeaponAnimationEvents: FirstPersonController를 찾을 수 없습니다. 부모 GameObject를 확인하세요.");
        }
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출 - 발소리 재생
    /// </summary>
    public void PlayFootstepSound()
    {
        if (controller != null)
        {
            controller.PlayFootstepSound();
        }
    }
}
