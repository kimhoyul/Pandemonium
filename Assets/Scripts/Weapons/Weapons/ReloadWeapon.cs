using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private Coroutine reloadWeaponCoroutine;

    private void Awake()
    {
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }

        reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(reloadWeaponEventArgs.weapon, reloadWeaponEventArgs.topUpAmmoPercent));
    }

    /// <summary>
    /// 무기 재장전 루틴
    /// </summary>
    /// <param name="weapon">재장전할 무기</param>
    /// <param name="topUpAmmoPercent">추가로 채울 탄약의 백분율</param>
    /// <returns>IEnumerator for Coroutine</returns>
    private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent)
    {
        // 무기를 재장전 중으로 표시
        weapon.isWeaponReloading = true;

        // 무기의 재장전 시간동안 대기
        while (weapon.weaponReloadTimer < weapon.weaponDetailsSO.weaponReloadTime)
        {
            weapon.weaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        // 추가 탄약 퍼센트가 0이 아니라면
        if (topUpAmmoPercent != 0)
        {
            // 탄약 증가량 계산
            int ammoIncrease = Mathf.RoundToInt((weapon.weaponDetailsSO.weaponAmmoCapacity * topUpAmmoPercent) / 100f);

            // 총 탄약 계산
            int totalAmmo = weapon.weaponRemainingAmmo + ammoIncrease;

            // 탄약이 무기의 최대 탄약 용량을 초과하면 최대 탄약으로 설정
            if (totalAmmo > weapon.weaponDetailsSO.weaponAmmoCapacity)
            {
                weapon.weaponRemainingAmmo = weapon.weaponDetailsSO.weaponAmmoCapacity;
            }
            else
            {
                weapon.weaponRemainingAmmo = totalAmmo;
            }
        }

        // 무기에 무한 탄약 옵션이 있으면
        if (weapon.weaponDetailsSO.hasInfiniteAmmo)
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetailsSO.weaponClipAmmoCapacity;
        }
        // 남은 탄약이 클립의 최대 탄약보다 많거나 같으면
        else if (weapon.weaponRemainingAmmo >= weapon.weaponDetailsSO.weaponClipAmmoCapacity)
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponDetailsSO.weaponClipAmmoCapacity;
        }
        // 그 외의 경우에는
        else
        {
            weapon.weaponClipRemainingAmmo = weapon.weaponRemainingAmmo;
        }

        // 재장전 타이머를 초기화
        weapon.weaponReloadTimer = 0f;

        // 재장전 중 표시를 해제
        weapon.isWeaponReloading = false;

        // 무기 재장전 이벤트 호출
        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }


    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.weapon.isWeaponReloading)
        {
            if (reloadWeaponCoroutine != null)
            {
                StopCoroutine(reloadWeaponCoroutine);
            }

            reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(setActiveWeaponEventArgs.weapon, 0));
        }
    }
}
