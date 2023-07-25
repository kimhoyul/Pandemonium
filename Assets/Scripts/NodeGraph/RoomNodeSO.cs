using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
	[HideInInspector] public RoomNodeGraphSO roomNodeGraph;
	public RoomNodeTypeSO roomNodeType;
	[HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

	#region Editor Code

#if UNITY_EDITOR

	[HideInInspector] public Rect rect;
	[HideInInspector] public bool isLeftClickDragging = false;
	[HideInInspector] public bool isSelected = false;

	public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
	{
		this.rect = rect;
		this.id = Guid.NewGuid().ToString();
		this.name = "RoomNode";
		this.roomNodeGraph = nodeGraph;
		this.roomNodeType = roomNodeType;

		roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	public void Draw(GUIStyle NodeStyle)
	{
		GUILayout.BeginArea(rect, NodeStyle);

		// 드롭박스의 변화를 감지 시작
		EditorGUI.BeginChangeCheck();

		if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
		{
			EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
		}
		else
		{
			int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

			int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

			roomNodeType = roomNodeTypeList.list[selection];

			if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isCorridor
				&& roomNodeTypeList.list[selection].isCorridor || !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
			{
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);

                        if (childRoomNode != null)
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }
            }
		}

		// 드롭박스의 변화를 감지 종료
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(this);

		GUILayout.EndArea();
	}

	// NodeGraphEditor에서 표시되어야 하는 RoomNodeTypes를 가져옴
	// RoomNodeTypeListSO의 displayInNodeGraphEditor가 true인 RoomNodeTypeSO만 가져옴
	private string[] GetRoomNodeTypesToDisplay()
	{
		string[] roomArray = new string[roomNodeTypeList.list.Count];

		for (int i = 0; i < roomNodeTypeList.list.Count; i++)
		{
			if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
			{
				roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
			}
		}

		return roomArray;
	}

	public void ProcessEvents(Event currentEvent)
	{
		switch (currentEvent.type)
		{
			case EventType.MouseDown:
				ProcessMouseDownEvent(currentEvent);
				break;
			case EventType.MouseUp:
				ProcessMouseUpEvent(currentEvent);
				break;
			case EventType.MouseDrag:
				ProcessMouseDragEvent(currentEvent);
				break;
			
			default:
				break;
		}
	}

	private void ProcessMouseDownEvent(Event currentEvent)
	{
		if (currentEvent.button == 0)
		{
			ProcessLeftClickDownEvent();
		}
		else if (currentEvent.button == 1)
		{
			ProcessRightClickDownEvent(currentEvent);
		}
	}

	private void ProcessLeftClickDownEvent()
	{
		Selection.activeObject = this;

		if (isSelected == true)
		{
			isSelected = false;
		}
		else
		{
			isSelected = true;
		}
	}

	private void ProcessRightClickDownEvent(Event currentEvent)
	{
		roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
	}

	private void ProcessMouseUpEvent(Event currentEvent)
	{
		if (currentEvent.button == 0)
		{
			ProcessLeftClickUpEvent();
		}
	}

	private void ProcessLeftClickUpEvent()
	{
		if (isLeftClickDragging)
		{
			isLeftClickDragging = false;
		}
	}

	private void ProcessMouseDragEvent(Event currentEvent)
	{
		if (currentEvent.button == 0)
		{
			ProcessLeftMouseDragEvent(currentEvent);
		}
	}

	private void ProcessLeftMouseDragEvent(Event currentEvent)
	{
		isLeftClickDragging = true;

		DragNode(currentEvent.delta);
		GUI.changed = true;
	}

	private void DragNode(Vector2 delta)
	{
		rect.position += delta;
		EditorUtility.SetDirty(this);
	}

	public bool AddChildRoomNodeIDToRoomNode(string childID)
	{
		if (IsChildRoomValid(childID))
		{
			childRoomNodeIDList.Add(childID);
			return true;
		}

		return false;
	}

	private bool IsChildRoomValid(string childID)
	{
		bool isConnectedBossNodeAlready = false;

		foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
		{
			// 보스름이면서 이미 연결된 부모가 있는지 확인
			if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
				isConnectedBossNodeAlready = true;
		}

		// child 노드의 ID로 조회하여 보스룸인지 그리고 이 이전에 연결된 보스룸이 있는지 확인
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
		{
			Debug.Log("Boss Room은 하나만 연결할 수 있습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 None인지 확인
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
		{
			Debug.Log("타입을 지정하지 않은 방(None)은 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 이미 연결된 자식인지 확인
		if (childRoomNodeIDList.Contains(childID))
		{
			Debug.Log("이미 연결된 자식 노드 입니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 자기 자신인지 확인
		if (id == childID)
		{
			Debug.Log("자기 자신과는 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 이미 연결된 부모인지 확인
		if (parentRoomNodeIDList.Contains(childID))
		{
			Debug.Log("이미 연결된 부모 노드 입니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 이미 다른 부모와 연결된 노드인지 확인
		if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
		{
			Debug.Log("다른 부모에 연결된 노드는 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 현재 노드의 타입이 복도이고 연결하려는 타입이 복도인지 확인
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
		{
			Debug.Log("Corridor 끼리는 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 현재 노드의 타입이 Room이고 연결하려는 타입이 Room인지 확인
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
		{
			Debug.Log("Room 끼리는 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 현재 연결하려는 노드의 타입이 복도이고 연결된 복도의 개수가 MaxChildCorridors를 초과하는지 확인
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.MaxChildCorridors)
		{
			Debug.Log("MaxChildCorridors를 초과하여 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 현재 연결하려는 노드의 타입이 시작지점인지 확인
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
		{
			Debug.Log("Entrance 는 최상위 부모 노드 이기에 연결할 수 없습니다.");
			return false;
		}

		// child 노드의 ID로 조회하여 현재 연결하려는 노드의 타입이 복도가 아니고 이미 차일드 노드를 갖고 있을 경우
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
		{
			Debug.Log("이미 연결된 Room 이 있습니다.");
			return false;
		}

		return true;
	}

	/// <summary>
	/// 현재 노드에 부모 노드를 추가.
	/// </summary>
	public bool AddParentRoomNodeIDToRoomNode(string parentID)
	{
		parentRoomNodeIDList.Add(parentID);
		return true;
	}

	/// <summary>
	/// 현재 노드에서 자식 노드를 제거.
	/// </summary>
	public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
	{
		if (childRoomNodeIDList.Contains(childID))
		{
			childRoomNodeIDList.Remove(childID);
			return true;
		}
		return false;
	}

    /// <summary>
    /// 현재 노드에서 부모 노드를 제거.
    /// </summary>
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}