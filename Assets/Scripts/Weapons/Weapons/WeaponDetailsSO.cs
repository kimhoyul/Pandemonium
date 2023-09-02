using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    [Tooltip("무기 이름")]
    public string weaponName;
    [Tooltip("무기의 스프라이트 - 스프라이트에 'generate physics shape' 옵션을 선택해주세요.")]
    public Sprite weaponSprite;

    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    [Tooltip("무기의 발사 위치")]
    public Vector3 weaponShootPosition;
    //[Tooltip("무기의 현재 탄약")]
    //public AmmoDetailsSO weaponCurrentAmmo;

    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    [Tooltip("무기가 무한한 탄약을 가지는지 여부를 선택하세요.")]
    public bool hasInfiniteAmmo = false;
    [Tooltip("무기가 무한한 탄창 용량을 가지는지 여부를 선택하세요.")]
    public bool hasInfiniteClipCapacity = false;
    [Tooltip("무기의 탄창 용량")]
    public int weaponClipAmmoCapacity = 6;
    [Tooltip("최대 소지 탄약 갯수")]
    public int weaponAmmoCapacity = 100;
    [Tooltip("무기 발사 속도")]
    public float weaponFireRate = 0.2f;
    [Tooltip("무기 사전 충전 시간 - 일정시간 누르고 있어야 하는 무기에 사용 됩니다.")]
    public float weaponPrechargeTime = 0f;
    [Tooltip("무기 재장전 시간")]
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR
       private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }
#endif
#endregion
}
