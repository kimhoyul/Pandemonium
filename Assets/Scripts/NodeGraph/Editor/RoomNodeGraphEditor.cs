using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoonNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;

	// Node Layout 값 설정
	private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Custom Editor Window 생성
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

	private void OnEnable()
	{
		// Node Style 설정
		roomNodeStyle = new GUIStyle();
		roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
		roomNodeStyle.normal.textColor = Color.white;
		roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
		roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // RoomNodeTypeListSO 가져오기
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
	}

	[OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
	    RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoonNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
	}

    // 윈도우창에 GUI 에 내용 그리기
    // update 함수와 비슷한 역할
	private void OnGUI()
    {
        if (currentRoonNodeGraph != null)
        {
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
	}

	private void ProcessEvents(Event currentEvent)
	{
		ProcessRoomNodeGraphEvents(currentEvent);
	}

	private void ProcessRoomNodeGraphEvents(Event currentEvent)
	{
		switch (currentEvent.type)
        {
			case EventType.MouseDown:
				ProcessMousedownEvent(currentEvent);
                break;

            default:
				break;
		}
	}

    private void ProcessMousedownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
			ShowContextMenu(currentEvent.mousePosition);
        }
	}

	private void ShowContextMenu(Vector2 mousePosition)
	{
		GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
	}

	private void CreateRoomNode(object mousePositionObject)
	{
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
	}

	private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
	{
        Vector2 mousePosition = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        currentRoonNodeGraph.roomNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoonNodeGraph, roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentRoonNodeGraph);

        AssetDatabase.SaveAssets();
	}

	private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoonNodeGraph.roomNodeList)
        {
			roomNode.Draw(roomNodeStyle);
		}

        GUI.changed = true;
   	}
}
