using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this, new FireWeaponEventArgs
        {
            fire = fire,
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponAimDirectionVector = weaponAimDirectionVector
        });
    }
}

public class FireWeaponEventArgs : EventArgs
{
    // 발사 여부
    public bool fire;
    // 목표 방향
    public AimDirection aimDirection;
    // 목표 각도 -> 플레이어 to 타겟 에서의 각도
    public float aimAngle;
    // 무기 목표 각도 -> WeaponShootPoint to 타겟 에서의 각도
    public float weaponAimAngle;
    // 무기 목표 방향 벡터 -> 플레이어와 타겟 사이의 거리에 따라 사용할예정. 위 두각도에 따라 계산된 벡터 저장
    public Vector3 weaponAimDirectionVector; 
} 