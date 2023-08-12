using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/던전/던전 룸 노드 그래프")]
public class RoomNodeGraphSO : ScriptableObject
{
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
	[HideInInspector] public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
	[HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

	private void Awake()
	{
		LoadRoomNodeDictionary();
	}

	private void LoadRoomNodeDictionary()
	{
		roomNodeDictionary.Clear();

		foreach (RoomNodeSO node in roomNodeList)
		{
			roomNodeDictionary[node.id] = node;
		}
	}

    // 일치하는 룸 노드 타입 검색
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
	{
        foreach (RoomNodeSO node in roomNodeList)
		{
            if (node.roomNodeType == roomNodeType)
			{
                return node;
            }
        }
        return null;
    }

	// 일치하는 룸 노드 ID 검색
	public RoomNodeSO GetRoomNode(string roomNodeID)
	{
		if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
		{
			return roomNode;
		}
		return null;
	}

	public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
	{
		foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
		{
			yield return GetRoomNode(childNodeID);
        }
    }

	#region Editor Code
#if UNITY_EDITOR
	[HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
	[HideInInspector] public Vector2 linePosition;
	public void OnValidate()
	{
		LoadRoomNodeDictionary();
	}

	public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
	{
		roomNodeToDrawLineFrom = node;
		linePosition = position;
	}
#endif
	#endregion Editor Code
}