using System;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
	#region Tooltip
	[Tooltip("weaponRotationPoint의 자녀 오브젝트의 transform 을 넣어주세요.")]
	#endregion
	[SerializeField] private Transform weaponRotationPointTransform;

	private AimWeaponEvent aimWeaponEvent;

	private void Awake()
	{
		aimWeaponEvent = GetComponent<AimWeaponEvent>();
	}

	private void OnEnable()
	{
		aimWeaponEvent.OnWeaponAIm += AimWeaponEvent_OnWeaponAim;
	}

	private void OnDisable()
	{
		aimWeaponEvent.OnWeaponAIm -= AimWeaponEvent_OnWeaponAim;
	}

	private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
	{
		Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
	}

	private void Aim(AimDirection aimDirection, float aimAngle)
	{
		weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

		switch (aimDirection)
		{
			case AimDirection.Left:
			case AimDirection.UpLeft:
				weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
				break;

			case AimDirection.Up:
			case AimDirection.UpRight:
			case AimDirection.Right:
			case AimDirection.Down:
				weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
				break;
		}
	}

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
	}
#endif
	#endregion
}
