using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public static class HelperUtilities
{
	public static Camera mainCamera;

	// 마우스의 월드 포지션 반환
	public static Vector3 GetMouseWorldPosition()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}

		Vector3 mouseScreenPosition = Input.mousePosition;

		// 마우스 위치를 화면 크기에 맞게 조절
		mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
		mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

		Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

		worldPosition.z = 0f;

		return worldPosition;
	}

	// 방향 벡터로부터 각도를 도 단위로 가져옴
	public static float GetAngleFromVector(Vector3 vector)
	{
		float radians = Mathf.Atan2(vector.y, vector.x);
		float degrees = radians * Mathf.Rad2Deg;

		return degrees;
	}

	// 각도 에서 AimDirection 열거형 값 가져오기
	public static AimDirection GetAimDirection(float angleDegrees)
	{
		AimDirection aimDirection;

		if (angleDegrees >= 22f && angleDegrees < 67f)
		{
			aimDirection = AimDirection.UpRight;
		}
		else if (angleDegrees > 67f && angleDegrees < 112f)
		{
			aimDirection = AimDirection.Up;
		}
		else if (angleDegrees > 112f && angleDegrees < 158f)
		{
			aimDirection = AimDirection.UpLeft;
		}
		else if ((angleDegrees <= 180f && angleDegrees > 158f) || (angleDegrees > -180f && angleDegrees <= -135f))
		{
			aimDirection = AimDirection.Left;
		}
		else if (angleDegrees > -135f && angleDegrees <= -45f)
		{
			aimDirection = AimDirection.Down;
		}
		else if ((angleDegrees > -45f && angleDegrees <= 0f) || (angleDegrees > 0f && angleDegrees < 22f))
		{
			aimDirection = AimDirection.Right;
		}
		else
		{
			aimDirection = AimDirection.Right;
		}

		return aimDirection;
	}

	// 빈 스트링 값이 있는지 확인
	public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
	{
		if (string.IsNullOrEmpty(stringToCheck))
		{
			Debug.Log("<b><color=yellow>" + thisObject.name.ToString() + "</color></b> 의 <b><color=yellow>" + fieldName + "</color></b>가 비어있습니다.");
			return false;
		}
		return true;
	}

	// 디버그중 null 값 확인
	public static bool ValidateCheckNullValue(Object thisObject, string fieldName, UnityEngine.Object objectToCheck)
	{
		if (objectToCheck == null)
		{
			Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>가 Null 입니다. 반드시 값이 포함되어야 합니다.");
			return true;
		}
		return false;
	}

	// 컬렉션에 값이 있는지 확인
	public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
	{
		bool error = false;
		int count = 0;

		if (enumerableObjectToCheck == null)
		{
			Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>가 비어있습니다.");
			return true;
		}

		foreach (var item in enumerableObjectToCheck)
		{
			if (item == null)
			{
				Debug.Log("<b><color=yellow>" + thisObject.name.ToString() + "</color></b> Scriptable Object의 <b><color=yellow>" + fieldName + "</color></b>에 Null 값이 있습니다.");
				error = true;
			}
			else
			{
				count++;
			}
		}

		if (count == 0)
		{
			Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>가 비어있습니다.");
			error = true;
		}

		return error;
	}

	// int 양수 값 확인 - 만약 제로가 허용되면 isZeroAllowed를 true 로 설정해야함
	public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, int valueToCheck, bool isZeroAllowed)
	{
		bool error = false;

		if (isZeroAllowed)
		{
			if (valueToCheck < 0)
			{
				Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>는 양수 값이나 0 이 포함되어야 합니다.");
				error = true;
			}
		}
		else
		{
			if (valueToCheck <= 0)
			{
				Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>는 양수 값이 포함되어야 합니다.");
				error = true;
			}
		}

		return error;
	}

	// float 양수 값 확인 - 만약 제로가 허용되면 isZeroAllowed를 true 로 설정해야함
	public static bool ValidateCheckPositiveValue(Object thisObject, string fieldName, float valueToCheck, bool isZeroAllowed)
	{
		bool error = false;

		if (isZeroAllowed)
		{
			if (valueToCheck < 0)
			{
				Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>는 양수 값이나 0 이 포함되어야 합니다.");
				error = true;
			}
		}
		else
		{
			if (valueToCheck <= 0)
			{
				Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldName}</color></b>는 양수 값이 포함되어야 합니다.");
				error = true;
			}
		}

		return error;
	}

    // float 범위 값 확인
    public static bool ValidateCheckPositiveRange(Object thisObject, string fieldNameMinimum, float valueToCheckMinimum, string fieldNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
	{
		bool error = false;
		if (valueToCheckMinimum > valueToCheckMaximum)
		{
			Debug.Log($"<b><color=yellow>{thisObject.name}</color></b> 의 <b><color=yellow>{fieldNameMinimum}</color></b> 은 <b><color=yellow>{fieldNameMaximum}</color></b>보다 작거나 같아야 합니다.");
			error = true;
		}

		if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
		{
            error = true;
        }

		if (ValidateCheckPositiveValue(thisObject, fieldNameMinimum, valueToCheckMinimum, isZeroAllowed))
		{
			error = true;
		}

		return error;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
	{
		Room currentRoom = GameManager.Instance.GetCurrentRoom();

		Grid grid = currentRoom.instantiatedRoom.grid;

		Vector3 nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

		foreach (Vector2Int spawnPositionGrid in currentRoom.spawnPositionArray)
		{
			Vector3 spawnPositionInWorldCoordinates = grid.CellToWorld((Vector3Int)spawnPositionGrid);

			if (Vector3.Distance(playerPosition, spawnPositionInWorldCoordinates) < Vector3.Distance(playerPosition, nearestSpawnPosition))
			{
				nearestSpawnPosition = spawnPositionInWorldCoordinates;
			}
		}

		return nearestSpawnPosition;
	}
}
