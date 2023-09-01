using System;
using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Tooltip("movementDetailsSO 는 속도와 같은 세부 정보를 포함하는 ScriptableObject 입니다.")]
    [SerializeField] private MovementDetailsSO movementDetails;

    [Tooltip("Player Prefab 의 hierarchy 에서 WeaponShootPosition gameobject 를 드래그 해 넣어주세요.")]
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
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

        SetPlayerAnimationSpeed();
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
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // 마우스 월드 포지션 계산
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        // 무기 발사 위치부터 마우스 까지의 방향 계산
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);

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
