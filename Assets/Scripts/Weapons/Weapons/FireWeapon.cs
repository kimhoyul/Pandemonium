using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    // 발사전 무기를 충전하는데 걸리는 시간
    private float firePreChargeTimer = 0f;
    // 무기를 발사한후 얼마나 시간이 지났는지 모니터링 하는 변수
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        // 무기 발사 속도 타이머 감소
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // 발사 충전 시간 핸들링
        WeaponPreChage(fireWeaponEventArgs);

        // 무기가 발사 됬는지 확인 -> PlayerControl 에서 이벤트를 호출
        if (fireWeaponEventArgs.fire)
        {
            /// 무기가 발사 준비 되었는지 확인
            if (IsWeaponReadyToFire())
            {
                // 무기 발사
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                // 발사한 후 쿨다운 타이머 초기화
                ResetCoolDownTimer();

                // 발사 충전 시간 초기화
                ResetPreChargeTimer();
            }
        }
    }

    private void WeaponPreChage(FireWeaponEventArgs fireWeaponEventArgs)
    {
        // 이전 프레임에서 발사 버튼을 눌렀다면
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            // firePreChargeTimer 감소
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            // 발사 준비 시간 초기화
            ResetPreChargeTimer();
        }
    }

    /// <summary>
    /// 무기가 발사 준비 되었는지 확인 하는 함수
    /// 무기에 탄약이 있는지, 무기 클립에 탄약이 있는지, 무기가 쿨다운 중인지 확인
    /// </summary>
    /// <returns></returns>
    private bool IsWeaponReadyToFire()
    {
        // 무기에 탄약이 있는지, 무기가 무한탄약인지 확인
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteAmmo)
            return false;

        // 무기가 재장전 중인지 확인
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        // 무기가 쿨다운 중이거나 발사 충전 시간이 남아 있는지 확인
        if (firePreChargeTimer > 0 || fireRateCoolDownTimer > 0)
            return false;

        // 무기가 탄창이 무한탄창 인지, 탄창에 탄약이 있는지 확인
        if (!activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            // 무한탄창이 아니고 탄창에 탄약도 없다면 재장전 이벤트 호출
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }

        // 무기가 발사 준비 되었음
        return true;
    }

    // 오브젝트 풀에서 가져온 Ammo GameObject 이용하여 세팅한다
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // 현재 활성화된 무기에서 발사할 탄약을 가져온다
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            // 현재 무기에 맞는 탄약에서 탄약 프리펩 배열을 조회하고 랜덤하게 하나를 가져옴
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            // ammoSpeedMin 과 ammoSpeedMax 사이의 랜덤한 총알의 속도
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            // Ammo 또는 AmmoPattern 에 구애 받지 않도록 IFireable 인터페이스를 구현한 클래스를 가져온다
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            // 총알 초기화
            ammo.InitializeAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            // 현재 무기가 무한탄창 인지 확인
            if (!activeWeapon.GetCurrentWeapon().weaponDetailsSO.hasInfiniteClipCapacity)
            {
                // 탄창에 남은 탄약 감소
                activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
                // 총 탄약에 남은 탄약 감소
                activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
            }

            // 무기 발사 이벤트 호출
            weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        }
    }

    /// <summary>
    /// 활성화된 무기의 발사 속도로 쿨다운 타이머를 재설정
    /// </summary>
    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponFireRate;
    }

    /// <summary>
    /// 발사 충전 시간 타이머를 재설정
    /// </summary>
    private void ResetPreChargeTimer()
    {
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetailsSO.weaponPrechargeTime;
    }

}
