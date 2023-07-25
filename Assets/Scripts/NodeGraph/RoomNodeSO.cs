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

		// ��ӹڽ��� ��ȭ�� ���� ����
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

		// ��ӹڽ��� ��ȭ�� ���� ����
		if (EditorGUI.EndChangeCheck())
			EditorUtility.SetDirty(this);

		GUILayout.EndArea();
	}

	// NodeGraphEditor���� ǥ�õǾ�� �ϴ� RoomNodeTypes�� ������
	// RoomNodeTypeListSO�� displayInNodeGraphEditor�� true�� RoomNodeTypeSO�� ������
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
			// �������̸鼭 �̹� ����� �θ� �ִ��� Ȯ��
			if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
				isConnectedBossNodeAlready = true;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���������� �׸��� �� ������ ����� �������� �ִ��� Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
		{
			Debug.Log("Boss Room�� �ϳ��� ������ �� �ֽ��ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� None���� Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
		{
			Debug.Log("Ÿ���� �������� ���� ��(None)�� ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� �̹� ����� �ڽ����� Ȯ��
		if (childRoomNodeIDList.Contains(childID))
		{
			Debug.Log("�̹� ����� �ڽ� ��� �Դϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� �ڱ� �ڽ����� Ȯ��
		if (id == childID)
		{
			Debug.Log("�ڱ� �ڽŰ��� ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� �̹� ����� �θ����� Ȯ��
		if (parentRoomNodeIDList.Contains(childID))
		{
			Debug.Log("�̹� ����� �θ� ��� �Դϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� �̹� �ٸ� �θ�� ����� ������� Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
		{
			Debug.Log("�ٸ� �θ� ����� ���� ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���� ����� Ÿ���� �����̰� �����Ϸ��� Ÿ���� �������� Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
		{
			Debug.Log("Corridor ������ ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���� ����� Ÿ���� Room�̰� �����Ϸ��� Ÿ���� Room���� Ȯ��
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
		{
			Debug.Log("Room ������ ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���� �����Ϸ��� ����� Ÿ���� �����̰� ����� ������ ������ MaxChildCorridors�� �ʰ��ϴ��� Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.MaxChildCorridors)
		{
			Debug.Log("MaxChildCorridors�� �ʰ��Ͽ� ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���� �����Ϸ��� ����� Ÿ���� ������������ Ȯ��
		if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
		{
			Debug.Log("Entrance �� �ֻ��� �θ� ��� �̱⿡ ������ �� �����ϴ�.");
			return false;
		}

		// child ����� ID�� ��ȸ�Ͽ� ���� �����Ϸ��� ����� Ÿ���� ������ �ƴϰ� �̹� ���ϵ� ��带 ���� ���� ���
		if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
		{
			Debug.Log("�̹� ����� Room �� �ֽ��ϴ�.");
			return false;
		}

		return true;
	}

	/// <summary>
	/// ���� ��忡 �θ� ��带 �߰�.
	/// </summary>
	public bool AddParentRoomNodeIDToRoomNode(string parentID)
	{
		parentRoomNodeIDList.Add(parentID);
		return true;
	}

	/// <summary>
	/// ���� ��忡�� �ڽ� ��带 ����.
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
    /// ���� ��忡�� �θ� ��带 ����.
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