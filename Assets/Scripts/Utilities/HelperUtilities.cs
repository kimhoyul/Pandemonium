using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
	public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
	{
		if (string.IsNullOrEmpty(stringToCheck))
		{
			Debug.Log("<b><color=yellow>" + thisObject.name.ToString() + "</color></b> �� <b><color=yellow>" + fieldName + "</color></b>�� ����ֽ��ϴ�.");
			return false;
		}
		return true;
	}

	public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
	{
		bool error = false;
		int count = 0;

		foreach (var item in enumerableObjectToCheck)
		{
			if (item == null)
			{
				Debug.Log("<b><color=yellow>" + thisObject.name.ToString() + "</color></b> Scriptable Object�� <b><color=yellow>" + fieldName + "</color></b>�� Null ���� �ֽ��ϴ�.");
				error = true;
			}
			else
			{
				count++;
			}
		}

		if (count == 0)
		{
			Debug.Log("<b><color=yellow>" + thisObject.name.ToString() + "</color></b> Scriptable Object�� <b><color=yellow>" + fieldName + "</color></b>�� ����ֽ��ϴ�.");
			error = true;
		}

		return error;
	}
}
