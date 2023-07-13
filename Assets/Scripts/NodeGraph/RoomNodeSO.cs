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

		EditorGUI.BeginChangeCheck();

		int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

		int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

		roomNodeType = roomNodeTypeList.list[selection];

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

#endif

	#endregion Editor Code
}