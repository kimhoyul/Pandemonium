using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
	public string roomNodeTypeName;

	#region Header
	[Header("Node Graph Editor�� ǥ�õǾ�� �ϴ� RoomNodeTypes���� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool displayInNodeGraphEditor = true;
	#region Header
	[Header("Type�� Corridor�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isCorridor;
	#region Header
	[Header("Type�� CorridorNS�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isCorridorNS;
	#region Header
	[Header("Type�� CorridorEW�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isCorridorEW;
	#region Header
	[Header("Type�� Entrance�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isEntrance;
	#region Header
	[Header("Type�� Boss Room�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isBossRoom;
	#region Header
	[Header("Type�� None�̸� �÷��׸� �����մϴ�.")]
	#endregion Header
	public bool isNone;

	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
	}
#endif
	#endregion
}
