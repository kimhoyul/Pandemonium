using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
	public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAIm;

	public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weapinAimAngle, Vector3 weaponAimDirectionVector)
	{
		OnWeaponAIm?.Invoke(this, new AimWeaponEventArgs()
		{
			aimDirection = aimDirection,
			aimAngle = aimAngle,
			weapinAimAngle = weapinAimAngle,
			weaponAimDirectionVector = weaponAimDirectionVector
		});
	}
}

public class AimWeaponEventArgs : EventArgs
{
	public AimDirection aimDirection;
	public float aimAngle;
	public float weapinAimAngle;
	public Vector3 weaponAimDirectionVector;
}