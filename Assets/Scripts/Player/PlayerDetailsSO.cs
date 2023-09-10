using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/PlayerDetailsSO")]
public class PlayerDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("PLAYER BASE DETAILS")]
    [Tooltip("플레이어의 이름을 입력하세요.")]
    public string playerCharacterName;

    [Tooltip("플레이어 프리펩을 넣어주세요.")]
    public GameObject playerPrefab;

    [Tooltip("플레이어의 runtime animator controller 를 널어주세요.")]
    public RuntimeAnimatorController runtimeAnimatorController;

    [Space(10)]
    [Header("HEALTH")]
    [Tooltip("플레이어의 체력을 입력하세요.")]
    public int playerHealthAmount;

    [Space(10)]
    [Header("WEAPON")]
    [Tooltip("플레이어의 초기 시작 무기")]
    public WeaponDetailsSO startingWeapon;
    [Tooltip("플레이어의 초기 시작 무기로 채워주세요.")]
    public List<WeaponDetailsSO> startingWeaponList;

    [Space(10)]
    [Header("OTHER")]
    [Tooltip("미니맵에서 사용할 플레이어 아이콘 스프라이트를 넣어주세요.")]
    public Sprite playerMinimapIcon;

    [Tooltip("플레이어 hand sprite 를 넣어주세요.")]
    public Sprite playerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMinimapIcon), playerMinimapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
    }
#endif
    #endregion
}
