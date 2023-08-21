using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyles guiStyles;

    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDreg;

    private RoomNodeSO currentRoomNode;
    private RoomNodeTypeListSO roomNodeTypeList;
    private List<RoomNodeSO> copyRoomNodeList;

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    /// UNITY Scripting API의 [OnOpenAsset] Attribute 를 사용
    /// instanceID 를 InstanceIDToObject 함수로 특정지어
    /// 해당 오브젝트가 RoomNodeGraphSO 이면 OpenWidow 함수를 호출
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }

    [MenuItem("던전 룸 노드 그래프 에디터", menuItem = "Window/던전 에디터/던전 룸 노드 그래프 에디터")]
    private static void OpenWindow()
    ///  UNITY Scripting API 의 [MenuItem] Attribute 를 사용
    /// 에디터 윈도우 오픈
    /// GerWindow<T>(”오픈된 윈도우의 타이틀에 들어갈 내용”)
    {
        GetWindow<RoomNodeGraphEditor>("던전 룸 노드 그래프 에디터");
    }

    private void OnEnable()
    {
        /// 에디터상에서 현재 활성/선택 항목이 변경되었을 때 트리거되는 델리게이트 → 구독
        Selection.selectionChanged += InspectorSelectionChanged;

        /// 선택된 노드 초기화
        currentRoomNode = null;

        /// GUIStyle 초기화
        guiStyles = new GUIStyles();

        /// GUIStyle 초기화
        guiStyles.Initialize();

        // RoomNodeTypeListSO 가져오기
        roomNodeTypeList = GameResources.Instance.typeList;

        // 노드 복사용 구조체 생성
        copyRoomNodeList = new List<RoomNodeSO>();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }

    // 윈도우창에 GUI 에 내용 그리기
    // update 함수와 비슷한 역할
    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(Settings.gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(Settings.gridLarge, 0.2f, Color.gray);

            DrawDraggedLine();

            ProcessEvents(Event.current);

            DrawRoomConnections();

            DrawRoomNodes(Event.current);
        }

        if (GUI.changed)
            Repaint();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDreg * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, Settings.connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        graphDreg = Vector2.zero;

        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentEvent.type == EventType.KeyDown || currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodeList.Count - 1; i >= 0; i--)
        {
            // rect.Contains는 rect안에 포인트가 있는지 확인함
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMousedownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.KeyDown:
                ProcessHotKeyEvent(currentEvent);
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
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("룸 노드 생성"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("선택된 룸 노드 연결 끊기"), false, DeleteSelectedRoomNodeLinks);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("모든 룸 노드 선택 [Ctrl + A]"), false, SelectAllRoomNodes);
        menu.AddItem(new GUIContent("선택된 룸 노드 복사 [Ctrl + C]"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("복사한 룸 노드 붙여넣기 [Ctrl + V]"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("선택된 룸 노드 삭제 [Delete]"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        // 현재 RoomNodeGraph 에 Room Node 가 없다면
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            // Entrance Node를 생성한다.
            CreateRoomNode(new Vector2(200, 200), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        // None Node를 생성한다.
        CreateRoomNode((Vector2)mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(Vector2 mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        // 스크립트블 오브젝트 생성(클래스는 RoomNodeSO)
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        // 생성한 룸노드를 현재 그래프의 리스트에 추가
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        
        float rectWidth = Settings.roomNodeWidth;
        float rectheight = Settings.roomNodeHeight;

        // 생성한 룸노드의 위치 와 타입을 설정
        roomNode.initialize(new Rect(mousePositionObject, new Vector2(rectWidth, rectheight)), currentRoomNodeGraph, roomNodeType);

        // 생성한 스크립트블 오브젝트를 룸 노드를 현재 그래프의 스크립트블 오브젝트의 하위 자식으로 추가한다.
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// 선택된 룸 노드를 삭제
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                foreach (string childRoonNodeID in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoonNodeID);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }

        while (roomNodeDeletionQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);

            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }

        ClearAllSelectedRoomNodes();
    }

    /// <summary>
    /// 선택된 룸 노드의 연결을 삭제
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);

                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
    }

    private void CopyRoomNodes()
    {
        copyRoomNodeList.Clear();

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                copyRoomNodeList.Add(roomNode);
            }
        }

        ClearAllSelectedRoomNodes();
    }

    private void PaseteRoomNodes(object mousePosition)
    {
        Vector2 temp = (Vector2)mousePosition;

        for (int i = 0; i < copyRoomNodeList.Count; i++)
        {
            if (i != 0)
            {
                temp += (copyRoomNodeList[i].rect.position - copyRoomNodeList[i-1].rect.position);
            }

            CreateRoomNode(temp, copyRoomNodeList[i].roomNodeType);
        }
    }

    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// 모든 룸 노드를 선택
    /// </summary>
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDreg = dragDelta;

        for (int i = 0; i < currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    private void ProcessHotKeyEvent(Event currentEvent)
    {
        // 모든 노드 선택
        if (currentEvent.keyCode == KeyCode.A && currentEvent.control)
        {
            SelectAllRoomNodes();
        }

        // 선택된 노드 삭제
        if (currentEvent.keyCode == KeyCode.Delete)
        {
            DeleteSelectedRoomNodes();
        }

        // 선택된 노드 복사
        if (currentEvent.keyCode == KeyCode.C && currentEvent.control)
        {
            CopyRoomNodes();
        }

        // 복사한 노드 붙여넣기
        if (currentEvent.keyCode == KeyCode.V && currentEvent.control)
        {
            PaseteRoomNodes(currentEvent.mousePosition);
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void DrawRoomConnections()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        // 선의 중간 점 찾기
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // 선의 방향벡터 구하기
        Vector2 direction = endPosition - startPosition;

        // 선의 중간점에서 화살표 날개로 생성할 점 구하기
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * Settings.connectingLineArrowSize; // 윗점
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * Settings.connectingLineArrowSize; // 아랫점

        // 선의 중간점에서 화살표 꼭짓점으로 생성할 점 구하기
        Vector2 arrowHeadPoint = midPosition + direction.normalized * Settings.connectingLineArrowSize;

        // 찾은 화살표 점들에 연결선 그리기
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, Settings.connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, Settings.connectingLineWidth);

        // 노드 연결선 그리기
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, Settings.connectingLineWidth);

        GUI.changed = true;
    }

    private void DrawRoomNodes(Event currentEvent)
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(GetRoomNodeStyle(roomNode));
        }

        GUI.changed = true;
    }

    private GUIStyle GetRoomNodeStyle(RoomNodeSO roomNode)
    {
        if (roomNode.roomNodeType.isEntrance)
        {
            return roomNode.isSelected ? guiStyles.entranceNodeSelectedStyle : guiStyles.entranceNodeStyle;
        }
        if (roomNode.roomNodeType.isCorridor)
        {
            return roomNode.isSelected ? guiStyles.corridorNodeSelectedStyle : guiStyles.corridorNodeStyle;
        }
        if (roomNode.roomNodeType.isBossRoom)
        {
            return roomNode.isSelected ? guiStyles.bossRoomNodeSelectedStyle : guiStyles.bossRoomNodeStyle;
        }
        if (roomNode.roomNodeType.isChestRoom)
        {
            return roomNode.isSelected ? guiStyles.chestRoomNodeSelectedStyle : guiStyles.chestRoomNodeStyle;
        }
        return roomNode.isSelected ? guiStyles.roomNodeSelectedStyle : guiStyles.roomNodeStyle;
    }
}

