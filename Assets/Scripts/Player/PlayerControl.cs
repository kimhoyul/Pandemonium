using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [Tooltip("movementDetailsSO 는 속도와 같은 세부 정보를 포함하는 ScriptableObject 입니다.")]
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        if (isPlayerRolling) return;

        MovementInput();

        WeaponInput();

        PlayerRollCooldownTimer();
    }

	private void Start()
	{
		waitForFixedUpdate = new WaitForFixedUpdate();

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
	}

    private void SetStartingWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if (weapon.weaponDetailsSO == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }

            index++;
        }
    }

    private void SetPlayerAnimationSpeed()
	{
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
	}

	private void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);


        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
				// Movement Event 호출
				player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
			}
            else if (playerRollCooldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
			}
        }
        else
        {
            // Idle Event 호출
            player.idleEvent.CallIdleEvent();
        }
    }

	private void PlayerRoll(Vector3 direction)
	{
		playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
	}

	private IEnumerator PlayerRollRoutine(Vector3 direction)
	{
		// minDistance는 코루틴 루프를 종료할 때 결정하는 데 사용됨
		float minDistance = 0.2f;

        isPlayerRolling = true;

        // Roll 이 끝나는 위치를 계산
        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

		// 현재 위치에서 targetPosition 까지의 거리가 minDistance 보다 크면 루프를 돌림
		while (Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

            yield return waitForFixedUpdate;
        }

		isPlayerRolling = false;

        playerRollCooldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;
	}

	private void PlayerRollCooldownTimer()
	{
		if (playerRollCooldownTimer > 0f)
        {
			playerRollCooldownTimer -= Time.deltaTime;
		}
	}

	private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // 마우스 월드 포지션 계산
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // 무기 발사 위치부터 마우스 까지의 방향 계산
        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        // 플레이어 위치부터 마우스 까지의 방향 계산
        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        // 무기의 방향 벡터를 각도로 변환
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        // 플레이어의 방향 벡터를 각도로 변환
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        // 플레이어의 방향 벡터를 AimDirection 으로 변환
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        // weapon aim event 호출
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
            leftMouseDownPreviousFrame = true;
        }
        else
        {
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        if (weaponIndex - 1 < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;

            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[currentWeaponIndex - 1]);
        }
    }

    private void ReloadWeaponInput()
    {
        // 현재 무기 가져오기
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();

        // 재장전 중인지 확인
        if (currentWeapon.isWeaponReloading) return;

        // 재장전 할 만큼 탄약이 있는지 확인 -> 현재 무기의 탄창 용량보다 탄약이 적거나, 무기가 무한 탄약을 가지고 있지 않은 경우 리턴
        if (currentWeapon.weaponRemainingAmmo < currentWeapon.weaponDetailsSO.weaponClipAmmoCapacity && !currentWeapon.weaponDetailsSO.hasInfiniteAmmo)
            return;

        // 탄창이 이미 꽉차있는지 확인
        if (currentWeapon.weaponClipRemainingAmmo == currentWeapon.weaponDetailsSO.weaponClipAmmoCapacity) return;

        // 재장전 키 입력 확인
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 재장전 이벤트 호출
            player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), 0);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
	{
        StopPlayerRollRoutine();
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		StopPlayerRollRoutine();
	}

	private void StopPlayerRollRoutine()
	{
		if (playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
	}

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
