using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [Tooltip("Weapon 게임 오브젝트의 SpriteRenderer 를 넣어주세요.")]
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [Tooltip("Weapon 게임 오브젝트의 PolygonCollider2D 를 넣어주세요.")]
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    [Tooltip("WeaponShootPosition 게임 오브젝트의 Transform 을 넣어주세요.")]
    [SerializeField] private Transform weaponShootPositionTransform;
    [Tooltip("WeaponEffectPosition 게임 오브젝트의 Transform 을 넣어주세요.")]
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        weaponSpriteRenderer.sprite = currentWeapon.weaponDetailsSO.weaponSprite;

        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            // 스프라이트의 물리 모양을 가져오기
            List<Vector2> spritePhysicsShapePointList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointList);

            // 물리 모양의 점들을 PolygonCollider2D 의 점들로 설정
            weaponPolygonCollider2D.points = spritePhysicsShapePointList.ToArray();
        }

        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetailsSO.weaponShootPosition;
    }

    public AmmoDetailsSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetailsSO.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider2D), weaponPolygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion
}
