using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
	public string roomNodeTypeName;

	#region Header
	[Header("Node Graph Editor에 표시되어야 하는 RoomNodeTypes에만 플래그를 지정합니다.")]
	#endregion Header
	public bool displayInNodeGraphEditor = true;
	#region Header
	[Header("Type이 Corridor이면 플래그를 지정합니다.")]
	#endregion Header
	public bool isCorridor;
	#region Header
	[Header("Type이 CorridorNS이면 플래그를 지정합니다.")]
	#endregion Header
	public bool isCorridorNS;
	#region Header
	[Header("Type이 CorridorEW이면 플래그를 지정합니다.")]
	#endregion Header
	public bool isCorridorEW;
	#region Header
	[Header("Type이 Entrance이면 플래그를 지정합니다.")]
	#endregion Header
	public bool isEntrance;
	#region Header
	[Header("Type이 Boss Room이면 플래그를 지정합니다.")]
	#endregion Header
	public bool isBossRoom;
	#region Header
	[Header("Type이 None이면 플래그를 지정합니다.")]
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
