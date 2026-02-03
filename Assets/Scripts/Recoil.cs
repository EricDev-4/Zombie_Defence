using UnityEngine;
using UnityEngine.InputSystem;

public class Recoil : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    
    
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;
    
    
    [Header("Hipfire Recoil")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    
    [Header("ADS Recoil")]
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;
    
    
    void Update()
    {
        
        playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero , returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if (playerInputHandler.AdsTriggered)
        {
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
        
    }
}
