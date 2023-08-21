using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public static class HelperUtilities
{

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

    // 양수 값 디버그 확인 - 만약 제로가 허용되면 isZeroAllowed를 true로 설정하고, 오류가 있는 경우 true를 반환
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
