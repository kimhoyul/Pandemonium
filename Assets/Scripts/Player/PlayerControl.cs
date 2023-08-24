using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Tooltip("movementDetailsSO 는 속도와 같은 세부 정보를 포함하는 ScriptableObject 입니다.")]
    [SerializeField] private MovementDetailsSO movementDetails;

    [Tooltip("Player Prefab 의 hierarchy 에서 WeaponShootPosition gameobject 를 드래그 해 넣어주세요.")]
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        }
        else
        {
            player.idleEvent.CallIdleEvent();
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

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
    #endregion
}
